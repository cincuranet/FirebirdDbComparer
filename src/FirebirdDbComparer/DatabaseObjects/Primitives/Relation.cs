using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Primitives
{
    [DebuggerDisplay("{MetadataRelationType}.{RelationName}")]
    public class Relation : Primitive<Relation>, IHasSystemFlag, IHasDescription
    {
        private static readonly EquatableProperty<Relation>[] s_EquatableProperties =
        {
            new EquatableProperty<Relation>(x => x.MetadataRelationType, nameof(MetadataRelationType)),
            new EquatableProperty<Relation>(x => x.RelationName, nameof(RelationName)),
            new EquatableProperty<Relation>(x => x.ViewSource, nameof(ViewSource)),
            new EquatableProperty<Relation>(x => x.ExternalFile, nameof(ExternalFile)),
            new EquatableProperty<Relation>(x => x.ExternalDescription, nameof(ExternalDescription)),
            new EquatableProperty<Relation>(x => x.OwnerName, nameof(OwnerName)),
            new EquatableProperty<Relation>(x => x.SystemFlag, nameof(SystemFlag)),
            new EquatableProperty<Relation>(x => x.Fields, nameof(Fields))
        };

        public Relation(ISqlHelper sqlHelper)
            : base(sqlHelper)
        { }

        public int RelationId { get; private set; }
        public MetadataRelationType MetadataRelationType { get; private set; }
        public Identifier RelationName { get; private set; }
        public DatabaseStringOrdinal Description { get; private set; }
        public DatabaseStringOrdinal ViewSource { get; private set; }
        public DatabaseStringOrdinal ExternalFile { get; private set; }
        public DatabaseStringOrdinal ExternalDescription { get; private set; }
        public DatabaseStringOrdinal OwnerName { get; private set; }
        public IList<RelationField> Fields { get; private set; }
        public SystemFlagType SystemFlag { get; private set; }
        public IList<Trigger> Triggers { get; set; }
        public IList<Trigger> UserTriggers { get; set; }
        public IList<RelationConstraint> RelationConstraints { get; set; }
        public IList<Index> Indices { get; set; }

        protected override Relation Self => this;

        protected override EquatableProperty<Relation>[] EquatableProperties => s_EquatableProperties;

        protected override IEnumerable<Command> OnCreate(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            var command =
                MetadataRelationType == MetadataRelationType.View
                    ? OnCreateView(sourceMetadata, targetMetadata, context)
                    : OnCreateTable(sourceMetadata, targetMetadata, context);
            yield return command;
        }

        protected virtual Command OnCreateTable(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            var command = new Command();
            var isGtt = MetadataRelationType == MetadataRelationType.GlobalTemporaryPreserve || MetadataRelationType == MetadataRelationType.GlobalTemporaryDelete;
            var isExternal = MetadataRelationType == MetadataRelationType.External;
            command.Append("CREATE ");
            if (isGtt)
            {
                command.Append("GLOBAL TEMPORARY ");
            }
            command.Append($"TABLE {RelationName.AsSqlIndentifier()} ");
            if (isExternal)
            {
                command.Append($"EXTERNAL '{SqlHelper.DoubleSingleQuotes(ExternalFile)}' ");
            }
            command.Append("(");
            command.AppendLine();
            InsertColumns(command, sourceMetadata, targetMetadata, context, false);
            command.AppendLine();
            command.Append(")");
            if (isGtt)
            {
                command.AppendLine();
                command.Append($"ON COMMIT {MetadataRelationType.ToDescription()} ROWS");
            }
            return command;
        }

        protected virtual Command OnCreateView(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            var command = new Command();
            command.Append($"CREATE OR ALTER VIEW {RelationName.AsSqlIndentifier()} (");
            command.AppendLine();
            InsertColumns(command, sourceMetadata, targetMetadata, context, true);
            command.Append(")");
            command.AppendLine();
            command.Append("AS");
            if (context.EmptyBodiesEnabled)
            {
                command.AppendLine();
                var fields = string.Join(", ", Fields.Select(x => SqlHelper.GetDataType(x.Field, sourceMetadata.MetadataCharacterSets.CharacterSetsById, sourceMetadata.MetadataDatabase.CharacterSet.CharacterSetId)).Select(RelationField.CreateShimFieldContent));
                command.Append($"SELECT {fields} FROM RDB$DATABASE");
            }
            else
            {
                if (!char.IsWhiteSpace(ViewSource[0]))
                {
                    command.AppendLine();
                }
                command.Append(ViewSource);
            }
            return command;
        }

        protected override IEnumerable<Command> OnDrop(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            yield return new Command()
                .Append($"DROP {(MetadataRelationType == MetadataRelationType.View ? "VIEW" : "TABLE")} {RelationName.AsSqlIndentifier()}");
        }

        protected override IEnumerable<Command> OnAlter(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            var commands =
                MetadataRelationType == MetadataRelationType.View
                    ? OnAlterView(sourceMetadata, targetMetadata, context)
                    : OnAlterTable(sourceMetadata, targetMetadata, context);
            return commands;
        }

        protected virtual IEnumerable<Command> OnAlterTable(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            var fieldNames = sourceMetadata
                .MetadataRelations
                .Relations[RelationName]
                .Fields
                .Select(f => f.FieldName);
            var otherFieldNames = targetMetadata
                .MetadataRelations
                .Relations[RelationName]
                .Fields
                .Select(f => f.FieldName);

            // 1. new fields
            var newFieldNames = new HashSet<Identifier>(fieldNames);
            newFieldNames.ExceptWith(otherFieldNames);
            var newFields = newFieldNames
                .Select(fieldName => sourceMetadata.MetadataRelations.RelationFields[new RelationFieldKey(RelationName, fieldName)]);
            foreach (var field in newFields)
            {
                foreach (var command in field.Create(sourceMetadata, targetMetadata, context))
                {
                    yield return command;
                }
                context.CreatedRelationFields.Add(new RelationFieldKey(field.RelationName, field.FieldName));
            }

            // 2. altered fields
            var alteredFieldNames = new HashSet<Identifier>(fieldNames);
            alteredFieldNames.IntersectWith(otherFieldNames);
            var alteredFields = alteredFieldNames
                .Select(fieldName => new
                                     {
                                         Field = sourceMetadata.MetadataRelations.RelationFields[new RelationFieldKey(RelationName, fieldName)],
                                         OtherField = targetMetadata.MetadataRelations.RelationFields[new RelationFieldKey(RelationName, fieldName)]
                                     })
                .Where(x => x.Field != x.OtherField)
                .Select(x => x.Field);
            foreach (var command in alteredFields.SelectMany(newField => newField.Alter(sourceMetadata, targetMetadata, context)))
            {
                yield return command;
            }

            // 3. dropped fields
            var droppedFieldNames = new HashSet<Identifier>(otherFieldNames);
            droppedFieldNames.ExceptWith(fieldNames);
            var droppedFields = droppedFieldNames
                .Select(fieldName => targetMetadata.MetadataRelations.RelationFields[new RelationFieldKey(RelationName, fieldName)]);
            foreach (var field in droppedFields)
            {
                context.DeferredColumnsToDrop.Add(new CommandGroup().Append(field.Drop(sourceMetadata, targetMetadata, context)));
            }
        }

        protected virtual IEnumerable<Command> OnAlterView(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            yield return OnCreateView(sourceMetadata, targetMetadata, context);
        }

        protected override Identifier OnPrimitiveTypeKeyObjectName() => TypeObjectNameKey.BuildObjectName(SqlHelper, MetadataRelationType, RelationName);

        private void InsertColumns(Command command, IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context, bool onlyNames)
        {
            var columns = Fields.OrderBy(x => x.FieldPosition).ToArray();
            command.Append(CreateColumnsDefinitions(columns, sourceMetadata, targetMetadata, context, onlyNames));
            context.CreatedRelationFields.AddRange(columns.Select(x => new RelationFieldKey(x.RelationName, x.FieldName)));
        }

        private static string CreateColumnsDefinitions(IEnumerable<RelationField> fields, IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context, bool onlyNames)
        {
            var builder = new StringBuilder();
            builder.Append("  ");
            var definitions = fields.Select(x => x.CreateDefinition(targetMetadata, sourceMetadata, context, onlyName: onlyNames));
            builder.Append(string.Join($",{Environment.NewLine}  ", definitions));
            return builder.ToString();
        }

        internal static Relation CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values, ILookup<Identifier, RelationField> relationFields)
        {
            var result =
                new Relation(sqlHelper)
                {
                    RelationId = values["RDB$RELATION_ID"].DbValueToInt32().GetValueOrDefault(),
                    MetadataRelationType = (MetadataRelationType)values["RDB$RELATION_TYPE"].DbValueToInt32().GetValueOrDefault(),
                    RelationName = new Identifier(sqlHelper, values["RDB$RELATION_NAME"].DbValueToString()),
                    Description = values["RDB$DESCRIPTION"].DbValueToString(),
                    ViewSource = values["RDB$VIEW_SOURCE"].DbValueToString(),
                    ExternalFile = values["RDB$EXTERNAL_FILE"].DbValueToString(),
                    ExternalDescription = values["RDB$EXTERNAL_DESCRIPTION"].DbValueToString(),
                    OwnerName = values["RDB$OWNER_NAME"].DbValueToString(),
                    SystemFlag = (SystemFlagType)values["RDB$SYSTEM_FLAG"].DbValueToInt32().GetValueOrDefault()
                };

            result.Fields = relationFields[result.RelationName].ToArray();

            return result;
        }
    }
}
