using System;
using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.DatabaseObjects.Elements;
using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Exceptions;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations
{
    public class MetadataRelations25 : DatabaseObject, IMetadataRelations, ISupportsComment
    {
        private IDictionary<Identifier, Relation> m_ExternalTables;
        private IDictionary<Identifier, Relation> m_GttTables;
        private IDictionary<RelationFieldKey, RelationField> m_RelationFields;
        private IDictionary<TypeObjectNameKey, RelationField> m_RelationFieldByTypeObjectNameKey;
        private IDictionary<Field, IList<RelationField>> m_RelationFieldsByField;
        private IDictionary<Identifier, Relation> m_Relations;
        private IDictionary<Identifier, Relation> m_Tables;
        private IList<ViewRelation> m_ViewRelations;
        private IDictionary<Identifier, Relation> m_Views;

        public MetadataRelations25(IMetadata metadata, ISqlHelper sqlHelper)
            : base(metadata, sqlHelper)
        { }

        protected virtual string RelationCommandText => @"
select R.RDB$RELATION_ID,
       R.RDB$RELATION_TYPE,
       trim(R.RDB$RELATION_NAME) as RDB$RELATION_NAME,
       R.RDB$DESCRIPTION,
       R.RDB$VIEW_SOURCE,
       R.RDB$EXTERNAL_FILE,
       R.RDB$EXTERNAL_DESCRIPTION,
       trim(R.RDB$OWNER_NAME) as RDB$OWNER_NAME,
       R.RDB$SYSTEM_FLAG
  from RDB$RELATIONS R";

        protected virtual string RelationFieldCommandText => @"
select trim(RF.RDB$RELATION_NAME) as RDB$RELATION_NAME,
       trim(RF.RDB$FIELD_NAME) as RDB$FIELD_NAME,
       trim(RF.RDB$FIELD_SOURCE) as RDB$FIELD_SOURCE,
       RF.RDB$FIELD_POSITION,
       trim(RF.RDB$BASE_FIELD) as RDB$BASE_FIELD,
       RF.RDB$VIEW_CONTEXT,
       RF.RDB$DESCRIPTION,
       iif(coalesce(RF.RDB$NULL_FLAG, 0) = 0, 0, 1) as RDB$NULL_FLAG,
       RF.RDB$DEFAULT_SOURCE,
       RF.RDB$COLLATION_ID,
       RF.RDB$SYSTEM_FLAG
  from RDB$RELATION_FIELDS RF";

        protected virtual string ViewRelationCommandText => @"
select trim(VR.RDB$VIEW_NAME) as RDB$VIEW_NAME,
       trim(VR.RDB$RELATION_NAME) as RDB$RELATION_NAME,
       VR.RDB$VIEW_CONTEXT,
       trim(VR.RDB$CONTEXT_NAME) as RDB$CONTEXT_NAME
  from RDB$VIEW_RELATIONS VR";

        public IDictionary<RelationFieldKey, RelationField> RelationFields => m_RelationFields;
        public IDictionary<TypeObjectNameKey, RelationField> RelationFieldByTypeObjectNameKey => m_RelationFieldByTypeObjectNameKey;
        public IDictionary<Identifier, Relation> Relations => m_Relations;
        public IDictionary<Identifier, Relation> Tables => m_Tables;
        public IDictionary<Identifier, Relation> Views => m_Views;
        public IDictionary<Identifier, Relation> ExternalTables => m_ExternalTables;
        public IDictionary<Identifier, Relation> GttTables => m_GttTables;
        public IList<ViewRelation> ViewRelations => m_ViewRelations;
        public IDictionary<Field, IList<RelationField>> RelationFieldsByField => m_RelationFieldsByField;

        public override void Initialize()
        {
            m_RelationFields =
                Execute(RelationFieldCommandText)
                    .Select(o => RelationField.CreateFrom(SqlHelper, o))
                    .ToDictionary(x => new RelationFieldKey(x.RelationName, x.FieldName));
            m_RelationFieldByTypeObjectNameKey =
                m_RelationFields
                    .Values
                    .ToDictionary(rf => rf.TypeObjectNameKey);
            var relationFields = m_RelationFields.Values.ToLookup(x => x.RelationName);
            m_Relations =
                Execute(RelationCommandText)
                    .Select(o => Relation.CreateFrom(SqlHelper, o, relationFields))
                    .ToDictionary(x => x.RelationName);
            m_ViewRelations =
                Execute(ViewRelationCommandText)
                    .Select(o => ViewRelation.CreateFrom(SqlHelper, o))
                    .ToList();
            m_Tables =
                m_Relations.Values
                    .Where(x => x.MetadataRelationType == MetadataRelationType.PERSISTENT)
                    .ToDictionary(x => x.RelationName);
            m_Views =
                m_Relations.Values
                    .Where(x => x.MetadataRelationType == MetadataRelationType.VIEW)
                    .ToDictionary(x => x.RelationName);
            m_ExternalTables =
                m_Relations.Values
                    .Where(x => x.MetadataRelationType == MetadataRelationType.EXTERNAL)
                    .ToDictionary(x => x.RelationName);
            m_GttTables =
                m_Relations.Values
                    .Where(x => x.MetadataRelationType == MetadataRelationType.GLOBAL_TEMPORARY_PRESERVE || x.MetadataRelationType == MetadataRelationType.GLOBAL_TEMPORARY_DELETE)
                    .ToDictionary(x => x.RelationName);
        }

        public override void FinishInitialization()
        {
            var lookup = ViewRelations.ToDictionary(vr => (viewName: vr.ViewName, viewContext: vr.ViewContext));
            foreach (var relationField in RelationFields.Values)
            {
                relationField.Relation = Relations[relationField.RelationName];
                relationField.Field = Metadata.MetadataFields.Fields[relationField.FieldSource];
                if (relationField.Relation.MetadataRelationType == MetadataRelationType.VIEW && relationField.BaseField != null)
                {
                    // ReSharper disable once PossibleInvalidOperationException
                    relationField.ViewRelation = lookup[(relationField.RelationName, (int)relationField.ViewContext)];
                }
                if (relationField.CollationId != null && relationField.Field.CharacterSetId != null)
                {
                    relationField.Collation = Metadata.MetadataCollations.CollationsByKey[new CollationKey((int)relationField.Field.CharacterSetId, (int)relationField.CollationId)];
                }
            }
            m_RelationFieldsByField = RelationFields.Values.ToMultiDictionary(x => x.Field);

            foreach (var relation in Relations.Values)
            {
                relation.Triggers =
                    Metadata.MetadataTriggers.TriggersByRelation.TryGetValue(relation.RelationName, out var triggers)
                        ? triggers
                        : new Trigger[0];

                relation.UserTriggers =
                    FilterSystemFlagUser(relation.Triggers)
                        .ToArray();

                relation.RelationConstraints =
                    Metadata.MetadataConstraints.RelationConstraintsByRelation.TryGetValue(relation.RelationName, out var constraints)
                        ? constraints
                        : new RelationConstraint[0];

                relation.Indices =
                    Metadata.MetadataIndices.IndicesByRelation.TryGetValue(relation.RelationName, out var indices)
                        ? indices
                        : new Index[0];
            }
        }

        public IEnumerable<CommandGroup> CreateTablesWithEmpty(IMetadata other, IComparerContext context)
        {
            return FilterNewTables(other)
                .Select(table => new CommandGroup().Append(WrapActionWithEmptyBody(table.Create)(Metadata, other, context)));
        }

        public IEnumerable<CommandGroup> AlterCreatedOrAlteredTablesToFull(IMetadata other, IComparerContext context)
        {
            return FilterNewTables(other).Concat(FilterTablesToBeAltered(other))
                .Select(table => new CommandGroup()
                            .Append(
                                table
                                    .Fields
                                    .Where(field => field.Field.ComputedSource != null)
                                    .Select(field => RelationField.AlterTableColumnHelper(field, field.CreateDefinition(Metadata, other, context, onlyDefinition: true)))
                                    .Select(x => new Command().Append(x))))
                .Where(x => !x.IsEmpty);
        }

        public IEnumerable<CommandGroup> AlterTablesAndToEmptyForAlteringOrDropping(IMetadata other, IComparerContext context)
        {
            foreach (var table in FilterTablesToBeAltered(other))
            {
                var dependencies = new CommandGroup();
                foreach (var field in table.Fields)
                {
                    var key = new RelationFieldKey(field.RelationName, field.FieldName);
                    if (other.MetadataRelations.RelationFields.TryGetValue(key, out var otherField) && field != otherField)
                    {
                        dependencies.Append(Metadata.MetadataConstraints.DropConstraintsForDependenciesHelper(s => new RelationFieldKey(s.RelationField.RelationName, s.RelationField.FieldName) == key, other, context));
                        dependencies.Append(Metadata.MetadataIndices.DropIndicesForDependenciesHelper(s => new RelationFieldKey(s.RelationField.RelationName, s.RelationField.FieldName) == key, other, context));
                    }
                }
                if (!dependencies.IsEmpty)
                {
                    yield return dependencies;
                }

                var result = new CommandGroup().Append(WrapActionWithEmptyBody(table.Alter)(Metadata, other, context));
                if (!result.IsEmpty)
                {
                    yield return result;
                }
            }
            foreach (var table in FilterTablesToBeDropped(other))
            {
                var result =
                    new CommandGroup()
                        .Append(
                            table
                                .Fields
                                .Where(field => field.Field.ComputedSource != null)
                                .SelectMany(field => WrapActionWithEmptyBody((m, o, c) => new[] { new Command().Append(RelationField.AlterTableColumnHelper(field, field.CreateDefinition(m, o, c, onlyDefinition: true))) })(Metadata, other, context)));
                if (!result.IsEmpty)
                {
                    yield return result;
                }
            }
        }

        public IEnumerable<CommandGroup> HandleTableFieldsPositions(IMetadata other, IComparerContext context)
        {
            var createdRelationFieldsByRelation =
                context
                    .CreatedRelationFields
                    .ToMultiDictionary(rfk => rfk.RelationName, rfk=> rfk.FieldName);

            var droppedRelationFieldsByRelation =
                context
                    .DroppedObjects
                    .Where(k => k.Type == typeof(RelationField))
                    .Select(k => other.MetadataRelations.RelationFieldByTypeObjectNameKey[k])
                    .ToMultiDictionary(rf => rf.RelationName, rf => rf.FieldName);

            var relations =
                GetAllTables(this)
                    .Where(r =>
                           {
                               createdRelationFieldsByRelation.TryGetValue(r.RelationName, out var createdFields);
                               createdFields = createdFields ?? Identifier.EmptyIdentifierList;

                               droppedRelationFieldsByRelation.TryGetValue(r.RelationName, out var droppedFields);
                               droppedFields = droppedFields ?? Identifier.EmptyIdentifierList;

                               var currentFields =
                                   r.Fields
                                       .OrderBy(x => x.FieldPosition)
                                       .Select(x => x.FieldName);

                               other.MetadataRelations.Relations.TryGetValue(r.RelationName, out var otherRelation);
                               if (otherRelation != null)
                               {
                                   return !otherRelation.Fields.OrderBy(x => x.FieldPosition).Select(x => x.FieldName)
                                              .Concat(createdFields)
                                              .Except(droppedFields)
                                              .SequenceEqual(currentFields);
                               }

                               return !currentFields.SequenceEqual(createdFields);
                           })
                    .OrderBy(r => r.RelationName);

            foreach (var relation in relations)
            {
                var fields = relation.Fields.OrderBy(f => f.FieldPosition);
                var definitions = fields.Select(f => $"ALTER COLUMN {f.FieldName.AsSqlIndentifier()} POSITION {f.FieldPosition + 1}");
                yield return new CommandGroup()
                    .Append(new Command()
                                .Append($"ALTER TABLE {relation.RelationName.AsSqlIndentifier()}")
                                .AppendLine()
                                .Append("  ")
                                .Append(string.Join($",{Environment.NewLine}  ", definitions)));
            }
        }

        public IEnumerable<CommandGroup> DropTables(IMetadata other, IComparerContext context)
        {
            return FilterTablesToBeDropped(other)
                .Select(table => new CommandGroup().Append(table.Drop(Metadata, other, context)));
        }

        public IEnumerable<CommandGroup> CreateEmptyViews(IMetadata other, IComparerContext context)
        {
            return FilterNewViews(other)
                .Select(view => new CommandGroup().Append(WrapActionWithEmptyBody(view.Create)(Metadata, other, context)));
        }

        public IEnumerable<CommandGroup> AlterViewsToFullBody(IMetadata other, IComparerContext context)
        {
            return FilterNewViews(other).Concat(FilterViewsToBeAltered(other))
                .Select(view => new CommandGroup().Append(view.Alter(Metadata, other, context)))
                .Where(x => !x.IsEmpty);
        }

        public IEnumerable<CommandGroup> AlterViewsToEmptyBodyForAlteringOrDropping(IMetadata other, IComparerContext context)
        {
            return FilterViewsToBeAltered(other).Concat(FilterViewsToBeDropped(other))
                .Select(view => new CommandGroup().Append(WrapActionWithEmptyBody(view.Alter)(Metadata, other, context)))
                .Where(x => !x.IsEmpty);
        }

        public IEnumerable<CommandGroup> DropViews(IMetadata other, IComparerContext context)
        {
            return FilterViewsToBeDropped(other)
                .Select(view => new CommandGroup().Append(view.Drop(Metadata, other, context)));
        }

        IEnumerable<CommandGroup> ISupportsComment.Handle(IMetadata other, IComparerContext context)
        {
            var actions = new[]
                          {
                              (predicate: (Func<Relation, bool>)(x => x.MetadataRelationType != MetadataRelationType.VIEW), name: "TABLE"),
                              (predicate: (Func<Relation, bool>)(x => x.MetadataRelationType == MetadataRelationType.VIEW), name: "VIEW"),
                          };
            foreach (var (predicate, name) in actions)
            {
                var result =
                    new CommandGroup()
                        .Append(
                            HandleComment(
                                Relations,
                                other.MetadataRelations.Relations,
                                x => x.RelationName,
                                name,
                                x => new[] { x.RelationName },
                                context,
                                primitivesFilter: predicate,
                                nestedFactory: x => HandleCommentNested(
                                                   x.Fields.OrderBy(y => y.FieldPosition),
                                                   other.MetadataRelations.RelationFields,
                                                   (a, b) => new RelationFieldKey(a, b),
                                                   x.RelationName,
                                                   y => y.FieldName,
                                                   "COLUMN",
                                                   y => new[] { y.FieldName },
                                                   context)));
                if (!result.IsEmpty)
                {
                    yield return result;
                }
            }
        }

        private IEnumerable<Relation> GetAllTables(IMetadataRelations metadataRelations)
        {
            return FilterSystemFlagUser(
                metadataRelations.Tables.Values
                    .Concat(metadataRelations.GttTables.Values)
                    .Concat(metadataRelations.ExternalTables.Values));
        }

        private IEnumerable<Relation> FilterNewTables(IMetadata other)
        {
            return GetAllTables(this)
                .Where(t =>
                       {
                           other.MetadataRelations.Relations.TryGetValue(t.RelationName, out var otherRelation);
                           if (otherRelation == null || otherRelation.MetadataRelationType != t.MetadataRelationType)
                           {
                               if (otherRelation != null && otherRelation.MetadataRelationType != t.MetadataRelationType)
                               {
                                   throw new CrossTypesOfSameObjectTypesException();
                               }
                               return true;
                           }
                           return false;
                       });
        }

        private IEnumerable<Relation> FilterTablesToBeDropped(IMetadata other)
        {
            return GetAllTables(other.MetadataRelations)
                .Where(t =>
                       {
                           Relations.TryGetValue(t.RelationName, out var relation);

                           return relation == null || relation.MetadataRelationType != t.MetadataRelationType;
                       });
        }

        private IEnumerable<Relation> FilterTablesToBeAltered(IMetadata other)
        {
            return GetAllTables(this)
                .Where(t =>
                       {
                           other.MetadataRelations.Relations.TryGetValue(t.RelationName, out var otherRelation);
                           return otherRelation != null && otherRelation.MetadataRelationType == t.MetadataRelationType && t != otherRelation;
                       });
        }

        private IEnumerable<Relation> FilterNewViews(IMetadata other)
        {
            return FilterSystemFlagUser(Views.Values)
                .Where(v => !other.MetadataRelations.Views.ContainsKey(v.RelationName));
        }

        private IEnumerable<Relation> FilterViewsToBeDropped(IMetadata other)
        {
            return FilterSystemFlagUser(other.MetadataRelations.Views.Values)
                .Where(v => !Views.ContainsKey(v.RelationName));
        }

        private IEnumerable<Relation> FilterViewsToBeAltered(IMetadata other)
        {
            return FilterSystemFlagUser(Views.Values)
                .Where(v => other.MetadataRelations.Views.TryGetValue(v.RelationName, out var otherView) && otherView != v);
        }
    }
}

