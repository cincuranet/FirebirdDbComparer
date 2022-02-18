using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Compare;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Primitives;

[DebuggerDisplay("{FieldName}")]
public sealed class Field : Primitive<Field>, IDataType, IHasSystemFlag, IHasDescription, IHasDefaultSource, IHasNullable, IHasValidationSource, IHasCollation
{
    private static readonly EquatableProperty<Field>[] s_EquatableProperties =
    {
            new EquatableProperty<Field>(x => x._EqualityFieldName, nameof(FieldName)),
            new EquatableProperty<Field>(x => x.ComputedSource, nameof(ComputedSource)),
            new EquatableProperty<Field>(x => x.DefaultSource, nameof(DefaultSource)),
            new EquatableProperty<Field>(x => x.ValidationSource, nameof(ValidationSource)),
            new EquatableProperty<Field>(x => x.FieldLength, nameof(FieldLength)),
            new EquatableProperty<Field>(x => x.Nullable, nameof(Nullable)),
            new EquatableProperty<Field>(x => x.SystemFlag, nameof(SystemFlag)),
            new EquatableProperty<Field>(x => x.MetadataFieldType, nameof(MetadataFieldType)),
            new EquatableProperty<Field>(x => x.CharacterSetId, nameof(CharacterSetId)),
            new EquatableProperty<Field>(x => x.FieldType, nameof(FieldType)),
            new EquatableProperty<Field>(x => x.FieldSubType, nameof(FieldSubType)),
            new EquatableProperty<Field>(x => x.FieldScale, nameof(FieldScale)),
            new EquatableProperty<Field>(x => x.FieldPrecision, nameof(FieldPrecision)),
            new EquatableProperty<Field>(x => x.SegmentSize, nameof(SegmentSize)),
            new EquatableProperty<Field>(x => x._EqualityCollationId, nameof(CollationId)),
            new EquatableProperty<Field>(x => x.OwnerName, nameof(OwnerName))
        };

    public Field(ISqlHelper sqlHelper)
        : base(sqlHelper)
    { }

    internal Identifier _EqualityFieldName => SqlHelper.HasSystemPrefix(FieldName) ? null : FieldName;
    public Identifier FieldName { get; private set; }
    public DatabaseStringOrdinal Description { get; private set; }
    public DatabaseStringOrdinal ComputedSource { get; private set; }
    public DatabaseStringOrdinal DefaultSource { get; private set; }
    public DatabaseStringOrdinal ValidationSource { get; private set; }
    public int? FieldLength { get; private set; }
    public bool Nullable { get; private set; }
    public SystemFlagType SystemFlag { get; private set; }
    public MetadataFieldType MetadataFieldType { get; private set; }
    public int? CharacterSetId { get; private set; }
    public CharacterSet CharacterSet { get; set; }
    public FieldType FieldType { get; private set; }
    public int? FieldSubType { get; private set; }
    public int? FieldScale { get; private set; }
    public int? FieldPrecision { get; private set; }
    public int? SegmentSize { get; private set; }
    public int? CharacterLength { get; private set; }
    // CORE-4934 and changing collation directly means modifying system table currently
    // system generated domains are sometimes explicit and sometimes relying on collation stored for the column/parameter directly
    internal int? _EqualityCollationId => ComputedSource != null || MetadataFieldType == MetadataFieldType.SystemGenerated ? null : CollationId;
    public int? CollationId { get; private set; }
    public Collation Collation { get; set; }
    public Identifier OwnerName { get; private set; }

    protected override Field Self => this;

    protected override EquatableProperty<Field>[] EquatableProperties => s_EquatableProperties;

    protected override IEnumerable<Command> OnCreate(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
    {
        var command = new Command();
        command
            .Append($"CREATE DOMAIN {FieldName.AsSqlIndentifier()} AS")
            .AppendLine()
            .Append(SqlHelper.GetDataType(this, sourceMetadata.MetadataCharacterSets.CharacterSetsById, sourceMetadata.MetadataDatabase.Database.CharacterSet.CharacterSetId));
        var defaultClause = SqlHelper.HandleDefault(this);
        if (defaultClause != null)
        {
            command
                .AppendLine()
                .Append(DefaultSource);
        }
        var notNullClause = SqlHelper.HandleNullable(this);
        if (notNullClause != null)
        {
            command
                .AppendLine()
                .Append(notNullClause);
        }
        var validationClause = SqlHelper.HandleValidation(this);
        if (validationClause != null)
        {
            command
                .AppendLine()
                .Append(validationClause);
        }
        var collateClause = SqlHelper.HandleCollate(this, sourceMetadata.MetadataCollations.CollationsByKey);
        if (collateClause != null)
        {
            command
                .AppendLine()
                .Append(collateClause);
        }
        yield return command;
    }

    protected override IEnumerable<Command> OnDrop(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
    {
        yield return new Command()
            .Append($"DROP DOMAIN {FieldName.AsSqlIndentifier()}");
    }

    protected override IEnumerable<Command> OnAlter(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
    {
        var otherField = FindOtherChecked(targetMetadata.MetadataFields.Fields, FieldName, "field");

        return
            SqlHelper.HandleAlterDefault(AlterDomainHelper, this, otherField)
                .Concat(SqlHelper.HandleAlterValidation(AlterDomainHelper, this, otherField))
                .Concat(SqlHelper.HandleAlterDataType(AlterDomainHelper, this, otherField, sourceMetadata, targetMetadata))
                .Concat(SqlHelper.HandleAlterCollation(FieldName, null, this, otherField))
                .Concat(SqlHelper.HandleAlterNullable(FieldName, null, this, otherField));
    }

    protected override Identifier OnPrimitiveTypeKeyObjectName() => FieldName;

    private string AlterDomainHelper(string action)
    {
        var builder = new StringBuilder();
        builder
            .Append($"ALTER DOMAIN {FieldName.AsSqlIndentifier()}")
            .AppendLine()
            .Append(action);
        return builder.ToString();
    }

    public static Field CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values)
    {
        var result =
            new Field(sqlHelper)
            {
                FieldName = new Identifier(sqlHelper, values["RDB$FIELD_NAME"].DbValueToString()),
                Description = values["RDB$DESCRIPTION"].DbValueToString(),
                ComputedSource = values["RDB$COMPUTED_SOURCE"].DbValueToString(),
                DefaultSource = values["RDB$DEFAULT_SOURCE"].DbValueToString(),
                ValidationSource = values["RDB$VALIDATION_SOURCE"].DbValueToString(),
                FieldType = (FieldType)values["RDB$FIELD_TYPE"].DbValueToInt32().GetValueOrDefault(),
                FieldSubType = values["RDB$FIELD_SUB_TYPE"].DbValueToInt32(),
                FieldLength = values["RDB$FIELD_LENGTH"].DbValueToInt32(),
                FieldScale = values["RDB$FIELD_SCALE"].DbValueToInt32(),
                SegmentSize = values["RDB$SEGMENT_LENGTH"].DbValueToInt32(),
                CharacterLength = values["RDB$CHARACTER_LENGTH"].DbValueToInt32(),
                CharacterSetId = values["RDB$CHARACTER_SET_ID"].DbValueToInt32(),
                CollationId = values["RDB$COLLATION_ID"].DbValueToInt32(),
                Nullable = values["RDB$NULL_FLAG"].DbValueToNullableFlag(),
                SystemFlag = (SystemFlagType)values["RDB$SYSTEM_FLAG"].DbValueToInt32().GetValueOrDefault()
            };
        result.FieldPrecision = AdjustFieldPrecision(values["RDB$FIELD_PRECISION"].DbValueToInt32(), result.FieldType);
        result.MetadataFieldType =
            result.SystemFlag == SystemFlagType.User && sqlHelper.HasSystemPrefix(result.FieldName)
                ? MetadataFieldType.SystemGenerated
                : MetadataFieldType.Domain;

        if (sqlHelper.TargetVersion.AtLeast(TargetVersion.Version30))
        {
            result.OwnerName = new Identifier(sqlHelper, values["RDB$OWNER_NAME"].DbValueToString());
        }
        return result;
    }

    private static int? AdjustFieldPrecision(int? fieldPrecision, FieldType fieldType)
    {
        int? result;
        if (fieldPrecision == null)
        {
            switch (fieldType)
            {
                case FieldType.Long:
                    result = 9;
                    break;
                case FieldType.Double:
                    result = 15;
                    break;
                default:
                    result = 18;
                    break;
            }
        }
        else
        {
            result = fieldPrecision;
        }
        return result;
    }
}
