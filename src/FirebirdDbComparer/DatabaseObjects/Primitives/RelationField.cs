using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Compare;
using FirebirdDbComparer.DatabaseObjects.Elements;
using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.Exceptions;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Primitives
{
    [DebuggerDisplay("{RelationName}.{FieldName}")]
    public sealed class RelationField : Primitive<RelationField>, IHasDescription, IHasDefaultSource, IHasNullable, IHasCollation, IUsesField
    {
        private static readonly EquatableProperty<RelationField>[] s_EquatableProperties =
        {
            new EquatableProperty<RelationField>(x => x.RelationName, nameof(RelationName)),
            new EquatableProperty<RelationField>(x => x.FieldName, nameof(FieldName)),
            new EquatableProperty<RelationField>(x => x.Field, nameof(Field)),
            new EquatableProperty<RelationField>(x => x.BaseField, nameof(BaseField)),
            new EquatableProperty<RelationField>(x => x.ViewContext, nameof(ViewContext)),
            new EquatableProperty<RelationField>(x => x.Nullable, nameof(Nullable)),
            new EquatableProperty<RelationField>(x => x.DefaultSource, nameof(DefaultSource)),
            new EquatableProperty<RelationField>(x => x._EqualityCollation, nameof(Collation)),
            new EquatableProperty<RelationField>(x => x.SystemFlag, nameof(SystemFlag)),
            new EquatableProperty<RelationField>(x => x.GeneratorName, nameof(GeneratorName)),
            new EquatableProperty<RelationField>(x => x.IdentityType, nameof(IdentityType))
        };

        public RelationField(ISqlHelper sqlHelper)
            : base(sqlHelper)
        { }

        public Identifier RelationName { get; private set; }
        public Identifier FieldName { get; private set; }
        public Identifier FieldSource { get; private set; }
        public int? FieldPosition { get; private set; }
        public string BaseField { get; private set; }
        public int? ViewContext { get; private set; }
        public DatabaseStringOrdinal Description { get; private set; }
        public bool Nullable { get; private set; }
        public DatabaseStringOrdinal DefaultSource { get; private set; }

        public int? CollationId { get; private set; }

        // CORE-4934 and changing collation directly means modifying system table currently
        // Sometimes even for same resulting structure (different combination of DDLs though) the collation is propagated and sometimes not
        internal Collation _EqualityCollation => Field?.ComputedSource != null ? null : Collation ?? Field._EqualityCollation;

        public Collation Collation { get; set; }
        public SystemFlagType SystemFlag { get; private set; }
        public Identifier GeneratorName { get; private set; }
        public IdentityTypeType IdentityType { get; private set; }
        public Field Field { get; set; }
        public ViewRelation ViewRelation { get; set; }
        public Relation Relation { get; set; }

        protected override RelationField Self => this;

        protected override EquatableProperty<RelationField>[] EquatableProperties => s_EquatableProperties;

        public string CreateDefinition(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context, bool onlyName = false, bool onlyDefinition = false)
        {
            var builder = new StringBuilder();
            if (!onlyDefinition)
            {
                builder.Append(FieldName.AsSqlIndentifier());
            }
            if (!onlyDefinition && !onlyName)
            {
                builder.Append(" ");
            }
            if (!onlyName)
            {
                var dataType = SqlHelper.GetDataType(this, sourceMetadata.MetadataCharacterSets.CharacterSetsById, sourceMetadata.MetadataDatabase.CharacterSet.CharacterSetId);
                if (Field.ComputedSource != null)
                {
                    if (context.EmptyBodiesEnabled)
                    {
                        builder.Append($"COMPUTED BY ({CreateShimFieldContent(dataType)})");
                    }
                    else
                    {
                        builder.Append($"COMPUTED BY {Field.ComputedSource}");
                    }
                }
                else
                {
                    builder.Append(dataType);
                    var defaultClause = SqlHelper.HandleDefault(this);
                    if (defaultClause != null)
                    {
                        builder.Append(" ");
                        builder.Append(defaultClause);
                    }
                    var notNullClause = SqlHelper.HandleNullable(this);
                    if (notNullClause != null)
                    {
                        builder.Append(" ");
                        builder.Append(notNullClause);
                    }
                    var collateClause = SqlHelper.HandleCollate(this, sourceMetadata.MetadataCollations.CollationsByKey);
                    if (collateClause != null)
                    {
                        builder.Append(" ");
                        builder.Append(collateClause);
                    }
                }
            }
            return builder.ToString();
        }

        protected override IEnumerable<Command> OnCreate(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            yield return new Command()
                .Append($"ALTER TABLE {RelationName.AsSqlIndentifier()} ADD {CreateDefinition(sourceMetadata, targetMetadata, context)}");
        }

        protected override IEnumerable<Command> OnDrop(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            yield return new Command()
                .Append($"ALTER TABLE {RelationName.AsSqlIndentifier()} DROP {FieldName.AsSqlIndentifier()}");
        }

        protected override IEnumerable<Command> OnAlter(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            var otherField = FindOtherChecked(targetMetadata.MetadataRelations.RelationFields, new RelationFieldKey(RelationName, FieldName), "column");
            if (Field.ComputedSource != null && otherField.Field.ComputedSource == null || Field.ComputedSource == null && otherField.Field.ComputedSource != null)
            {
                throw new NotSupportedOnFirebirdException($"Altering from computed to normal field or visa versa is not supported ({RelationName}.{FieldName}).");
            }

            if (Field.ComputedSource != null)
            {
                if (context.EmptyBodiesEnabled)
                {
                    var dataType = SqlHelper.GetDataType(this, targetMetadata.MetadataCharacterSets.CharacterSetsById, targetMetadata.MetadataDatabase.CharacterSet.CharacterSetId);
                    yield return new Command().Append(AlterTableColumnHelper($"COMPUTED BY ({CreateShimFieldContent(dataType)})"));
                }
                else
                {
                    yield return new Command().Append(AlterTableColumnHelper($"COMPUTED BY {Field.ComputedSource}"));
                }
            }
            else
            {
                var commands = SqlHelper.HandleAlterDefault(AlterTableColumnHelper, this, otherField)
                    .Concat(SqlHelper.HandleAlterDataType(AlterTableColumnHelper, this, otherField, sourceMetadata, targetMetadata))
                    .Concat(SqlHelper.HandleAlterCollation(FieldName, RelationName, this, otherField))
                    .Concat(SqlHelper.HandleAlterNullable(FieldName, RelationName, this, otherField));
                foreach (var command in commands)
                {
                    yield return command;
                }
            }
        }

        protected override Identifier OnPrimitiveTypeKeyObjectName() => TypeObjectNameKey.BuildObjectName(SqlHelper, RelationName, FieldName);

        private string AlterTableColumnHelper(string action)
        {
            return AlterTableColumnHelper(this, action);
        }

        public static string AlterTableColumnHelper(RelationField field, string action)
        {
            var builder = new StringBuilder();
            builder
                .Append($"ALTER TABLE {field.RelationName.AsSqlIndentifier()} ALTER {field.FieldName.AsSqlIndentifier()} ")
                .Append(action);
            return builder.ToString();
        }

        public static string CreateShimFieldContent(string dataType)
        {
            return $"CAST(NULL AS {dataType})";
        }

        internal static RelationField CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values)
        {
            var result =
                new RelationField(sqlHelper)
                {
                    RelationName = new Identifier(sqlHelper, values["RDB$RELATION_NAME"].DbValueToString()),
                    FieldName = new Identifier(sqlHelper, values["RDB$FIELD_NAME"].DbValueToString()),
                    FieldSource = new Identifier(sqlHelper, values["RDB$FIELD_SOURCE"].DbValueToString()),
                    FieldPosition = values["RDB$FIELD_POSITION"].DbValueToInt32(),
                    BaseField = values["RDB$BASE_FIELD"].DbValueToString(),
                    ViewContext = values["RDB$VIEW_CONTEXT"].DbValueToInt32(),
                    Description = values["RDB$DESCRIPTION"].DbValueToString(),
                    Nullable = values["RDB$NULL_FLAG"].DbValueToNullable(),
                    DefaultSource = values["RDB$DEFAULT_SOURCE"].DbValueToString(),
                    CollationId = values["RDB$COLLATION_ID"].DbValueToInt32(),
                    SystemFlag = (SystemFlagType)values["RDB$SYSTEM_FLAG"].DbValueToInt32().GetValueOrDefault()
                };

            if (sqlHelper.TargetVersion.AtLeast30())
            {
                result.GeneratorName = new Identifier(sqlHelper, values["RDB$GENERATOR_NAME"].DbValueToString());
                result.IdentityType = (IdentityTypeType)values["RDB$IDENTITY_TYPE"].DbValueToInt32().GetValueOrDefault();
            }
            return result;
        }
    }
}
