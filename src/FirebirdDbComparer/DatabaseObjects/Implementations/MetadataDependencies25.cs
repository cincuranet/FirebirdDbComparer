using System;
using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.DatabaseObjects.Elements;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Implementations
{
    public class MetadataDependencies25 : DatabaseObject, IMetadataDependencies
    {
        private List<Dependency> m_Dependencies;
        private IDictionary<Identifier, IList<Dependency>> m_DependedOnNames;
        private IDictionary<Identifier, IList<Dependency>> m_DependentNames;

        public MetadataDependencies25(IMetadata metadata, ISqlHelper sqlHelper)
            : base(metadata, sqlHelper)
        { }

        public List<Dependency> Dependencies => m_Dependencies;

        public IDictionary<Identifier, IList<Dependency>> DependedOnNames => m_DependedOnNames;

        public IDictionary<Identifier, IList<Dependency>> DependentNames => m_DependentNames;

        protected virtual string CommandText => @"
select trim(D.RDB$DEPENDENT_NAME) as RDB$DEPENDENT_NAME,
       trim(D.RDB$DEPENDED_ON_NAME) as RDB$DEPENDED_ON_NAME,
       trim(D.RDB$FIELD_NAME) as RDB$FIELD_NAME,
       D.RDB$DEPENDENT_TYPE,
       D.RDB$DEPENDED_ON_TYPE
 from RDB$DEPENDENCIES D";

        public override void Initialize()
        {
            m_Dependencies = Execute(CommandText)
                .Select(o => Dependency.CreateFrom(SqlHelper, o))
                .ToList();
            m_DependentNames = m_Dependencies.ToMultiDictionary(d => d.DependentName);
            m_DependedOnNames = m_Dependencies.ToMultiDictionary(d => d.DependedOnName);
        }

        public override void FinishInitialization()
        {
            foreach (var dependency in Dependencies)
            {
                switch (dependency.DependentType)
                {
                    case ObjectType.RELATION:
                    case ObjectType.VIEW:
                        dependency.DependentRelation =
                            Metadata
                                .MetadataRelations
                                .Relations[dependency.DependentName];
                        break;
                    case ObjectType.TRIGGER:
                        dependency.DependentTrigger =
                            Metadata
                                .MetadataTriggers
                                .TriggersByName[dependency.DependentName];
                        break;
                    case ObjectType.FIELD:
                    case ObjectType.COMPUTED_FIELD:
                        dependency.DependentField =
                            Metadata
                                .MetadataFields
                                .Fields[dependency.DependentName];
                        break;
                    case ObjectType.PROCEDURE:
                        dependency.DependentProcedure =
                            Metadata
                                .MetadataProcedures
                                .ProceduresByName[dependency.DependentName];
                        break;
                    case ObjectType.EXCEPTION:
                        dependency.DependentException =
                            Metadata
                                .MetadataExceptions
                                .ExceptionsByName[dependency.DependentName];
                        break;
                    case ObjectType.ROLE:
                        dependency.DependentRole =
                            Metadata
                                .MetadataRoles
                                .Roles[dependency.DependentName];
                        break;
                    case ObjectType.UDF:
                        dependency.DependentFunction =
                            Metadata
                                .MetadataFunctions
                                .FunctionsByName[dependency.DependentNameKey];
                        break;
                    case ObjectType.EXPRESSION_INDEX:
                        dependency.DependentIndex =
                            Metadata
                                .MetadataIndices
                                .Indices[dependency.DependentName];
                        break;
                }

                switch (dependency.DependedOnType)
                {
                    case ObjectType.RELATION:
                    case ObjectType.VIEW:
                        dependency.DependedOnRelation =
                            Metadata
                                .MetadataRelations
                                .Relations[dependency.DependedOnName];
                        break;
                    case ObjectType.TRIGGER:
                        dependency.DependedOnTrigger =
                            Metadata
                                .MetadataTriggers
                                .TriggersByName[dependency.DependedOnName];
                        break;
                    case ObjectType.FIELD:
                    case ObjectType.COMPUTED_FIELD:
                        dependency.DependedOnField =
                            Metadata
                                .MetadataFields
                                .Fields[dependency.DependedOnName];
                        break;
                    case ObjectType.PROCEDURE:
                        dependency.DependedOnProcedure =
                            Metadata
                                .MetadataProcedures
                                .ProceduresByName[dependency.DependedOnName];
                        break;
                    case ObjectType.EXCEPTION:
                        dependency.DependedOnException =
                            Metadata
                                .MetadataExceptions
                                .ExceptionsByName[dependency.DependedOnName];
                        break;
                    case ObjectType.ROLE:
                        dependency.DependedOnRole =
                            Metadata
                                .MetadataRoles
                                .Roles[dependency.DependedOnName];
                        break;
                    case ObjectType.UDF:
                        dependency.DependedOnFunction =
                            Metadata
                                .MetadataFunctions
                                .FunctionsByName[dependency.DependedOnNameKey];
                        break;
                    case ObjectType.EXPRESSION_INDEX:
                        dependency.DependendOnIndex =
                            Metadata
                                .MetadataIndices
                                .Indices[dependency.DependedOnName];
                        break;
                }
            }
        }

        public TreeNode GetDependenciesFor(ITypeObjectNameKey primitiveTypeKey)
        {
            var visitedObjects = new HashSet<ITypeObjectNameKey>();
            var root = new TreeNode(primitiveTypeKey, GetDirectDependenciesFor(primitiveTypeKey));
            visitedObjects.Add(primitiveTypeKey);
            GetTreeDependenciesFor(root, visitedObjects);
            return root;
        }

        private void GetTreeDependenciesFor(TreeNode parent, HashSet<ITypeObjectNameKey> visitedObjects)
        {
            if (parent != null)
            {
                foreach (var primitiveTypeKey in parent.Dependencies)
                {
                    if (visitedObjects.Add(primitiveTypeKey))
                    {
                        var child = new TreeNode(primitiveTypeKey, GetDirectDependenciesFor(primitiveTypeKey));
                        parent.Nodes.Add(child);
                        GetTreeDependenciesFor(child, visitedObjects);
                    }
                }
            }
        }

        private HashSet<ITypeObjectNameKey> GetDirectDependenciesFor(ITypeObjectNameKey primitiveTypeKey)
        {
            var result = new HashSet<ITypeObjectNameKey>();

            if (primitiveTypeKey is Field field)
            {
                result.AddRange(GetDependenciesFor(field));
            }
            if (primitiveTypeKey is Relation relation)
            {
                result.AddRange(GetDependenciesFor(relation));
            }
            if (primitiveTypeKey is RelationField relationField)
            {
                result.AddRange(GetDependenciesFor(relationField));
            }
            if (primitiveTypeKey is Procedure procedure)
            {
                result.AddRange(GetDependenciesFor(procedure));
            }
            if (primitiveTypeKey is Trigger trigger)
            {
                result.AddRange(GetDependenciesFor(trigger));
            }
            if (primitiveTypeKey is DbException dbException)
            {
                result.AddRange(GetDependenciesFor(dbException));
            }
            if (primitiveTypeKey is Function function)
            {
                result.AddRange(GetDependenciesFor(function));
            }
            if (primitiveTypeKey is Generator generator)
            {
                result.AddRange(GetDependenciesFor(generator));
            }
            if (primitiveTypeKey is Index index)
            {
                result.AddRange(GetDependenciesFor(index));
            }

            return result;
        }

        private IEnumerable<ITypeObjectNameKey> GetDependencies(Identifier dependendOnName, Func<Dependency, bool> predicate)
        {
            if (Metadata.MetadataDependencies.DependedOnNames.TryGetValue(dependendOnName, out var dependedOnNames))
            {
                foreach (var dependency in dependedOnNames.Where(predicate))
                {
                    yield return dependency;

                    switch (dependency.DependentType)
                    {
                        case ObjectType.RELATION:
                        case ObjectType.VIEW:
                            yield return dependency.DependentRelation;
                            break;
                        case ObjectType.TRIGGER:
                            if (dependency.DependentTrigger.SystemFlag == SystemFlagType.USER)
                            {
                                yield return dependency.DependentTrigger;
                            }
                            break;
                        case ObjectType.FIELD:
                        case ObjectType.COMPUTED_FIELD:
                            yield return dependency.DependentField;
                            break;
                        case ObjectType.PROCEDURE:
                            yield return dependency.DependentProcedure;
                            break;
                        case ObjectType.EXPRESSION_INDEX:
                            yield return dependency.DependentIndex;
                            break;
                        case ObjectType.EXCEPTION:
                            yield return dependency.DependentException;
                            break;
                        case ObjectType.UDF:
                            yield return dependency.DependentFunction;
                            break;
                    }
                }
            }
        }

        private IEnumerable<ITypeObjectNameKey> GetDependenciesFor(Field field)
        {
            foreach (var dependency in GetDependencies(field.FieldName, _ => true))
            {
                yield return dependency;
            }

            var relationFields =
                Metadata
                    .MetadataRelations
                    .RelationFieldsByField[field];
            foreach (var relationField in relationFields)
            {
                yield return relationField;
            }

            var procedureParams =
                Metadata
                    .MetadataProcedures
                    .ProcedureParameters
                    .Values
                    .Where(p => p.Field == field);
            foreach (var parameter in procedureParams)
            {
                yield return parameter;
            }

            var relationContraints =
                Metadata
                    .MetadataConstraints
                    .RelationConstraintsByName
                    .Values
                    .Where(c => (c.RelationConstraintType == RelationConstraintType.FOREIGN_KEY
                                 || c.RelationConstraintType == RelationConstraintType.PRIMARY_KEY
                                 || c.RelationConstraintType == RelationConstraintType.UNIQUE)
                                && c.Index != null && c.Index.Segments.Any(s => s.RelationField.Field == field));
            foreach (var relationConstraint in relationContraints)
            {
                yield return relationConstraint;
            }

            var indices =
                Metadata
                    .MetadataIndices
                    .Indices
                    .Values
                    .Where(i => i.IsUserCreatedIndex && i.Segments.Any(s => s.RelationField.Field == field));
            foreach (var index in indices)
            {
                yield return index;
            }
        }

        private IEnumerable<ITypeObjectNameKey> GetDependenciesFor(Relation relation)
        {
            foreach (var dependency in GetDependencies(relation.RelationName, _ => true))
            {
                yield return dependency;
            }

            foreach (var relationField in relation.Fields)
            {
                yield return relationField;
            }

            foreach (var relationConstraint in relation.RelationConstraints)
            {
                yield return relationConstraint;
            }

            foreach (var index in relation.Indices.Where(i => i.IsUserCreatedIndex))
            {
                yield return index;
            }
        }

        private IEnumerable<ITypeObjectNameKey> GetDependenciesFor(RelationField relationField)
        {
            foreach (var dependency in GetDependencies(relationField.RelationName, d => d.FieldName == relationField.FieldName))
            {
                yield return dependency;
            }

            var indices =
                Metadata
                    .MetadataIndices
                    .Indices
                    .Values
                    .Where(i => i.IsUserCreatedIndex && i.Segments.Any(s => s.RelationField == relationField));
            foreach (var index in indices)
            {
                yield return index;
            }
        }

        private IEnumerable<ITypeObjectNameKey> GetDependenciesFor(Procedure procedure)
        {
            return GetDependencies(procedure.ProcedureName, _ => true);
        }

        private IEnumerable<ITypeObjectNameKey> GetDependenciesFor(Trigger trigger)
        {
            return GetDependencies(trigger.TriggerName, _ => true);
        }

        private IEnumerable<ITypeObjectNameKey> GetDependenciesFor(DbException dbException)
        {
            return GetDependencies(dbException.ExceptionName, _ => true);
        }

        private IEnumerable<ITypeObjectNameKey> GetDependenciesFor(Function function)
        {
            return GetDependencies(function.FunctionName, _ => true);
        }

        private IEnumerable<ITypeObjectNameKey> GetDependenciesFor(Generator generator)
        {
            return GetDependencies(generator.GeneratorName, _ => true);
        }

        private IEnumerable<ITypeObjectNameKey> GetDependenciesFor(Index index)
        {
            return GetDependencies(index.IndexName, _ => true);
        }
    }
}
