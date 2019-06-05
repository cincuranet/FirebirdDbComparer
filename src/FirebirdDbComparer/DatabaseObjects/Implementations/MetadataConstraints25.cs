using System;
using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.DatabaseObjects.Elements;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations
{
    public class MetadataConstraints25 : DatabaseObject, IMetadataConstraints
    {
        private IList<CheckConstraint> m_CheckConstraints = new List<CheckConstraint>();
        private IDictionary<Identifier, ReferenceConstraint> m_ReferenceConstraintsByName;
        private IDictionary<Identifier, IList<ReferenceConstraint>> m_ReferenceConstraintsByNameUq;
        private IDictionary<Identifier, IList<ReferenceConstraint>> m_ReferenceConstraintsByRelation;
        private IDictionary<Identifier, RelationConstraint> m_RelationConstraintsByName;
        private IDictionary<Identifier, RelationConstraint> m_RelationConstraintsByIndexName;
        private IDictionary<Identifier, IList<RelationConstraint>> m_RelationConstraintsByRelation;

        public MetadataConstraints25(IMetadata metadata, ISqlHelper sqlHelper)
            : base(metadata, sqlHelper)
        { }

        public IList<CheckConstraint> CheckConstraints => m_CheckConstraints;

        public IDictionary<Identifier, ReferenceConstraint> ReferenceConstraintsByName => m_ReferenceConstraintsByName;

        public IDictionary<Identifier, IList<ReferenceConstraint>> ReferenceConstraintsByNameUq => m_ReferenceConstraintsByNameUq;

        public IDictionary<Identifier, IList<ReferenceConstraint>> ReferenceConstraintsByRelation => m_ReferenceConstraintsByRelation;

        public IDictionary<Identifier, RelationConstraint> RelationConstraintsByName => m_RelationConstraintsByName;

        public IDictionary<Identifier, RelationConstraint> RelationConstraintsByIndexName => m_RelationConstraintsByIndexName;

        public IDictionary<Identifier, IList<RelationConstraint>> RelationConstraintsByRelation => m_RelationConstraintsByRelation;

        protected virtual string CheckConstraintsCommandText => @"
select trim(CC.RDB$CONSTRAINT_NAME) as RDB$CONSTRAINT_NAME,
       trim(CC.RDB$TRIGGER_NAME) as RDB$TRIGGER_NAME
  from RDB$CHECK_CONSTRAINTS CC";

        protected virtual string ReferenceConstraintsCommandText => @"
select trim(RC.RDB$CONSTRAINT_NAME) as RDB$CONSTRAINT_NAME,
       trim(RC.RDB$CONST_NAME_UQ) as RDB$CONST_NAME_UQ,
       trim(RC.RDB$UPDATE_RULE) as RDB$UPDATE_RULE,
       trim(RC.RDB$DELETE_RULE) as RDB$DELETE_RULE
  from RDB$REF_CONSTRAINTS RC";

        protected virtual string RelationConstraintsCommandText => @"
select trim(RC.RDB$CONSTRAINT_NAME) as RDB$CONSTRAINT_NAME,
       trim(RC.RDB$CONSTRAINT_TYPE) as RDB$CONSTRAINT_TYPE,
       trim(RC.RDB$RELATION_NAME) as RDB$RELATION_NAME,
       trim(RC.RDB$INDEX_NAME) as RDB$INDEX_NAME
  from RDB$RELATION_CONSTRAINTS RC";

        public override void Initialize()
        {
            m_CheckConstraints =
                Execute(CheckConstraintsCommandText)
                    .Select(o => CheckConstraint.CreateFrom(SqlHelper, o))
                    .ToList();
            m_ReferenceConstraintsByName =
                Execute(ReferenceConstraintsCommandText)
                    .Select(o => ReferenceConstraint.CreateFrom(SqlHelper, o))
                    .ToDictionary(x => x.ConstraintName);
            m_RelationConstraintsByName =
                Execute(RelationConstraintsCommandText)
                    .Select(o => RelationConstraint.CreateFrom(SqlHelper, o))
                    .ToDictionary(x => x.ConstraintName);

            m_ReferenceConstraintsByNameUq = m_ReferenceConstraintsByName.Values
                .ToMultiDictionary(x => x.ConstraintNameUq);
            m_ReferenceConstraintsByRelation = m_ReferenceConstraintsByName.Values
                .ToMultiDictionary(x => m_RelationConstraintsByName[x.ConstraintName].RelationName);
            m_RelationConstraintsByIndexName = m_RelationConstraintsByName.Values
                .Where(x => x.IndexName != null)
                .ToDictionary(x => x.IndexName);

            m_RelationConstraintsByRelation = m_RelationConstraintsByName.Values
                .ToMultiDictionary(x => x.RelationName);
        }

        public override void FinishInitialization()
        {
            var checkConstraintsByName = CheckConstraints.ToLookup(c => c.ConstraintName);

            foreach (var constraint in RelationConstraintsByName.Values)
            {
                if (constraint.RelationConstraintType == RelationConstraintType.NotNull)
                {
                    constraint.FieldName =
                        checkConstraintsByName[constraint.ConstraintName]
                            .Select(c => c.TriggerName)
                            .Single();
                }
                else if (constraint.RelationConstraintType == RelationConstraintType.Check)
                {
                    constraint.Triggers =
                        checkConstraintsByName[constraint.ConstraintName]
                            .Select(c => Metadata.MetadataTriggers.TriggersByName[c.TriggerName])
                            .ToArray();
                }
                constraint.Relation =
                    Metadata
                        .MetadataRelations
                        .Relations[constraint.RelationName];
                if (constraint.IndexName != null)
                {
                    constraint.Index = Metadata.MetadataIndices.Indices[constraint.IndexName];
                }
            }

            foreach (var constraint in ReferenceConstraintsByName.Values)
            {
                constraint.Triggers =
                    checkConstraintsByName[constraint.ConstraintName]
                        .Select(c => Metadata.MetadataTriggers.TriggersByName[c.TriggerName])
                        .ToArray();
                constraint.RelationConstraint =
                    Metadata
                        .MetadataConstraints
                        .RelationConstraintsByName[constraint.ConstraintName];
                constraint.RelationConstraintUq =
                    Metadata
                        .MetadataConstraints
                        .RelationConstraintsByName[constraint.ConstraintNameUq];
            }
        }

        public IEnumerable<CommandGroup> HandleConstraints(IMetadata other, IComparerContext context)
        {
            var batch = new CommandGroup();

            batch.Append(DropCheckConstraints(other, context));
            batch.Append(CreateCheckConstraints(other, context));
            batch.Append(DropConstraints(ForeignKeyConstraintPredicate, other, context));
            batch.Append(DropConstraints(PrimaryKeyConstraintPredicate, other, context));
            batch.Append(DropConstraints(UniqueConstraintPredicate, other, context));
            batch.Append(CreateConstraints(UniqueConstraintPredicate, other, context));
            batch.Append(CreateConstraints(PrimaryKeyConstraintPredicate, other, context));
            batch.Append(CreateConstraints(ForeignKeyConstraintPredicate, other, context));
            batch.Append(RecreateConstraintsDroppedAsReferenceConstraint(other, context));

            if (!batch.IsEmpty)
            {
                yield return batch;
            }
        }

        public IEnumerable<Command> DropConstraintsForDependenciesHelper(Func<IndexSegment, bool> selector, IMetadata other, IComparerContext context)
        {
            var relationConstraints =
                other
                    .MetadataConstraints
                    .RelationConstraintsByName
                    .Values
                    .Where(c => (c.RelationConstraintType == RelationConstraintType.PrimaryKey
                                 || c.RelationConstraintType == RelationConstraintType.Unique
                                 || c.RelationConstraintType == RelationConstraintType.ForeignKey)
                                && c.Index.Segments.Any(selector))
                    .Where(c => !context.DroppedObjects.Contains(c.TypeObjectNameKey))
                    .OrderBy(c => c.RelationConstraintType == RelationConstraintType.PrimaryKey
                                  || c.RelationConstraintType == RelationConstraintType.Unique);
            foreach (var relationConstraint in relationConstraints)
            {
                if (relationConstraint.RelationConstraintType == RelationConstraintType.PrimaryKey || relationConstraint.RelationConstraintType == RelationConstraintType.Unique)
                {
                    if (other.MetadataConstraints.ReferenceConstraintsByNameUq.TryGetValue(relationConstraint.ConstraintName, out var referenceConstraints))
                    {
                        var dependencies = referenceConstraints
                            .Select(rc => other.MetadataConstraints.ReferenceConstraintsByName[rc.ConstraintName])
                            .Where(rc => !context.DroppedObjects.Contains(rc.RelationConstraint.TypeObjectNameKey));
                        foreach (var command in dependencies.SelectMany(d => d.RelationConstraint.Drop(Metadata, other, context)))
                        {
                            yield return command;
                        }
                    }
                }

                foreach (var command in relationConstraint.Drop(Metadata, other, context))
                {
                    yield return command;
                }
            }
        }

        private bool ConstraintPredicate(
            IMetadataConstraints otherConstraints,
            RelationConstraint c,
            RelationConstraintType relationConstraintType,
            Func<RelationConstraint, bool> predicate)
        {
            if (c.RelationConstraintType != relationConstraintType)
            {
                return false;
            }
            if (!FilterSystemFlagUserPredicate(c.Relation))
            {
                return false;
            }
            if (relationConstraintType != RelationConstraintType.ForeignKey && !Metadata.MetadataRelations.Relations.ContainsKey(c.RelationName))
            {
                return false;
            }

            var otherConstraint = otherConstraints.RelationConstraintsByName.Values.FirstOrDefault(x => x == c);
            if (otherConstraint != null)
            {
                return predicate == null
                           ? otherConstraint != c
                           : otherConstraint != c || predicate(otherConstraint);
            }

            return true;
        }

        private IEnumerable<Command> DropConstraints(Func<IMetadataConstraints, IMetadataConstraints, RelationConstraint, bool> predicate, IMetadata other, IComparerContext context)
        {
            var constraintsToBeDropped =
                other
                    .MetadataConstraints
                    .RelationConstraintsByName
                    .Values
                    .Where(c => predicate(other.MetadataConstraints, Metadata.MetadataConstraints, c))
                    .Where(c => !context.DroppedObjects.Contains(c.TypeObjectNameKey))
                    .OrderBy(rc => rc.RelationName)
                    .ThenBy(rc => rc.ConstraintName);
            foreach (var constraint in constraintsToBeDropped)
            {
                if (constraint.RelationConstraintType == RelationConstraintType.PrimaryKey || constraint.RelationConstraintType == RelationConstraintType.Unique)
                {
                    if (ReferenceConstraintsByNameUq.TryGetValue(constraint.ConstraintName, out var referenceConstraints))
                    {
                        var dependencies =
                            referenceConstraints
                                .Select(rc => ReferenceConstraintsByName[rc.ConstraintName])
                                .Where(rc => other.MetadataConstraints.ReferenceConstraintsByName.ContainsKey(rc.ConstraintName))
                                .Where(rc => !context.DroppedObjects.Contains(rc.TypeObjectNameKey));
                        foreach (var item in dependencies.SelectMany(d => d.RelationConstraint.Drop(Metadata, other, context)))
                        {
                            yield return item;
                        }
                    }
                    else
                    {
                        var constraintFields = constraint.Index.Segments.Select(x => x.RelationField.TypeObjectNameKey);
                        if (constraintFields.All(x => context.DroppedObjects.Contains(x)))
                        {
                            continue;
                        }
                    }
                }
                foreach (var item in constraint.Drop(Metadata, other, context))
                {
                    yield return item;
                }
            }
        }

        private IEnumerable<Command> CreateConstraints(Func<IMetadataConstraints, IMetadataConstraints, RelationConstraint, bool> predicate, IMetadata other, IComparerContext context)
        {
            var commands =
                RelationConstraintsByName
                    .Values
                    .Where(rc => predicate(Metadata.MetadataConstraints, other.MetadataConstraints, rc))
                    .OrderBy(rc => rc.RelationName)
                    .ThenBy(rc => rc.ConstraintName)
                    .SelectMany(c => c.Create(Metadata, other, context));

            return commands;
        }

        private IEnumerable<Command> RecreateConstraintsDroppedAsReferenceConstraint(IMetadata other, IComparerContext context)
        {
            var relationConstraintsToBeRecreated =
                RelationConstraintsByName
                    .Values
                    .Where(
                        rc =>
                        {
                            other.MetadataConstraints.RelationConstraintsByName.TryGetValue(rc.ConstraintName, out var otherRelationConstraint);
                            return
                                (rc.RelationConstraintType == RelationConstraintType.ForeignKey
                                 || rc.RelationConstraintType == RelationConstraintType.PrimaryKey
                                 || rc.RelationConstraintType == RelationConstraintType.Unique)
                                && context.DroppedObjects.Contains(rc.TypeObjectNameKey)
                                && otherRelationConstraint != null
                                && otherRelationConstraint == rc;
                        })
                    .OrderBy(rc =>
                             {
                                 switch (rc.RelationConstraintType)
                                 {
                                     case RelationConstraintType.PrimaryKey:
                                         return 1;
                                     case RelationConstraintType.Unique:
                                         return 2;
                                     default:
                                         return 3;
                                 }
                             })
                    .ThenBy(rc => rc.RelationName)
                    .ThenBy(rc => rc.ConstraintName);
            return relationConstraintsToBeRecreated.SelectMany(c => c.Create(Metadata, other, context));
        }

        private IEnumerable<Command> CreateCheckConstraints(IMetadata other, IComparerContext context)
        {
            return DoCheckConstraints(Metadata, other, x => x.Create, context);
        }

        private IEnumerable<Command> DropCheckConstraints(IMetadata other, IComparerContext context)
        {
            return DoCheckConstraints(other, Metadata, x => x.Drop, context);
        }

        private bool ForeignKeyConstraintPredicate(IMetadataConstraints constraints, IMetadataConstraints otherConstraints, RelationConstraint c)
        {
            return ConstraintPredicate(otherConstraints, c, RelationConstraintType.ForeignKey,
                                       otherConstraint =>
                                           !otherConstraints.RelationConstraintsByName.Values.Contains(c)
                                           || !otherConstraints.ReferenceConstraintsByName.Values.Contains(constraints.ReferenceConstraintsByName[c.ConstraintName]));
        }

        private bool PrimaryKeyConstraintPredicate(IMetadataConstraints constraints, IMetadataConstraints otherConstraints, RelationConstraint c)
        {
            return ConstraintPredicate(otherConstraints, c, RelationConstraintType.PrimaryKey, null);
        }

        private bool UniqueConstraintPredicate(IMetadataConstraints constraints, IMetadataConstraints otherConstraints, RelationConstraint c)
        {
            return ConstraintPredicate(otherConstraints, c, RelationConstraintType.Unique, null);
        }

        private static IEnumerable<Command> DoCheckConstraints(IMetadata left, IMetadata right, Func<RelationConstraint, Func<IMetadata, IMetadata, IComparerContext, IEnumerable<Command>>> doAction, IComparerContext context)
        {
            var rightCheckConstraints = FilterCheckRelationConstraintsConstraints(right.MetadataConstraints)
                .ToList();
            foreach (var leftCheckConstraint in FilterCheckRelationConstraintsConstraints(left.MetadataConstraints))
            {
                var index = rightCheckConstraints.FindIndex(x => RelationConstraint.CheckConstraintComparer.Equals(x, leftCheckConstraint));
                if (index != -1)
                {
                    rightCheckConstraints.RemoveAt(index);
                }
                else
                {
                    foreach (var item in doAction(leftCheckConstraint)(right, left, context))
                    {
                        yield return item;
                    }
                }
            }
        }

        private static IEnumerable<RelationConstraint> FilterCheckRelationConstraintsConstraints(IMetadataConstraints metadataConstraints)
        {
            return metadataConstraints.RelationConstraintsByName.Values
                .Where(x => x.RelationConstraintType == RelationConstraintType.Check);
        }
    }
}
