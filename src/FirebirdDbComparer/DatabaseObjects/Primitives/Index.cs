using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.DatabaseObjects.Elements;
using FirebirdDbComparer.Exceptions;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Primitives;

[DebuggerDisplay("{RelationName}.{IndexName}")]
public sealed class Index : Primitive<Index>, IHasSystemFlag, IHasDescription
{
    private static readonly EquatableProperty<Index>[] s_EquatableProperties =
    {
            new EquatableProperty<Index>(x => x.IndexName, nameof(IndexName)),
            new EquatableProperty<Index>(x => x.RelationName, nameof(RelationName)),
            new EquatableProperty<Index>(x => x.Unique, nameof(Unique)),
            new EquatableProperty<Index>(x => x.SegmentCount, nameof(SegmentCount)),
            new EquatableProperty<Index>(x => x.Inactive, nameof(Inactive)),
            new EquatableProperty<Index>(x => x.Descending, nameof(Descending)),
            new EquatableProperty<Index>(x => x._EqualityForeignKey, nameof(ForeignKey)),
            new EquatableProperty<Index>(x => x.ExpressionSource, nameof(ExpressionSource)),
            new EquatableProperty<Index>(x => x.Segments, nameof(Segments)),
            new EquatableProperty<Index>(x => x.SystemFlag, nameof(SystemFlag))
        };

    public Index(ISqlHelper sqlHelper)
        : base(sqlHelper)
    { }

    public Identifier IndexName { get; private set; }
    public Identifier RelationName { get; private set; }
    public bool Unique { get; private set; }
    public DatabaseStringOrdinal Description { get; private set; }
    public int? SegmentCount { get; private set; }
    public bool Inactive { get; private set; }
    public bool Descending { get; private set; }
    internal Identifier _EqualityForeignKey => ForeignKey == null || SqlHelper.HasSystemPrefix(ForeignKey) ? null : ForeignKey;
    public Identifier ForeignKey { get; private set; }
    public DatabaseStringOrdinal ExpressionSource { get; private set; }
    public IList<IndexSegment> Segments { get; private set; }
    public SystemFlagType SystemFlag { get; private set; }
    public RelationConstraint RelationConstraint { get; set; }
    public Relation Relation { get; set; }

    public bool IsUserCreatedIndex => RelationConstraint == null && SystemFlag == SystemFlagType.User;

    public bool CanAlter(Index other) => EquatableHelper.PropertiesEqual(this, other, EquatableProperties, nameof(Inactive));

    protected override Index Self => this;

    protected override EquatableProperty<Index>[] EquatableProperties => s_EquatableProperties;

    protected override IEnumerable<Command> OnCreate(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
    {
        if (IsUserCreatedIndex)
        {
            var command = new Command();
            command
                .Append($"CREATE {(Unique ? "UNIQUE " : string.Empty)}{(Descending ? "DESCENDING" : "ASCENDING")} INDEX {IndexName.AsSqlIndentifier()}")
                .AppendLine()
                .Append($"    ON {RelationName.AsSqlIndentifier()}");
            if (ExpressionSource == null)
            {
                command.Append($"({string.Join(",", Segments.Select(s => s.FieldName.AsSqlIndentifier()))})");
            }
            else
            {
                command.Append($" COMPUTED BY {ExpressionSource}");
            }

            yield return command;

            if (Inactive)
            {
                yield return HandleIndexActiveInactive(context);
            }
        }
    }

    protected override IEnumerable<Command> OnDrop(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
    {
        if (IsUserCreatedIndex)
        {
            yield return new Command()
                .Append($"DROP INDEX {IndexName.AsSqlIndentifier()}");
        }
    }

    protected override IEnumerable<Command> OnAlter(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
    {
        if (!IsUserCreatedIndex)
        {
            yield break;
        }

        var otherIndex = FindOtherChecked(targetMetadata.MetadataIndices.Indices, IndexName, "index");

        if (CanAlter(otherIndex))
        {
            yield return HandleIndexActiveInactive(context);
        }
        else
        {
            throw new DeadCodePathException();
        }
    }

    protected override Identifier OnPrimitiveTypeKeyObjectName() => IndexName;

    private Command HandleIndexActiveInactive(IComparerContext context)
    {
        return new Command()
            .Append($"ALTER INDEX {IndexName.AsSqlIndentifier()} {(Inactive ? "INACTIVE" : "ACTIVE")}");
    }

    internal static Index CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values, ILookup<Identifier, IndexSegment> indexSegments)
    {
        var result =
            new Index(sqlHelper)
            {
                IndexName = new Identifier(sqlHelper, values["RDB$INDEX_NAME"].DbValueToString()),
                RelationName = new Identifier(sqlHelper, values["RDB$RELATION_NAME"].DbValueToString()),
                Unique = values["RDB$UNIQUE_FLAG"].DbValueToBool().GetValueOrDefault(),
                Description = values["RDB$DESCRIPTION"].DbValueToString(),
                SegmentCount = values["RDB$SEGMENT_COUNT"].DbValueToInt32(),
                Inactive = values["RDB$INDEX_INACTIVE"].DbValueToBool().GetValueOrDefault(),
                Descending = values["RDB$INDEX_TYPE"].DbValueToBool().GetValueOrDefault(),
                ForeignKey = new Identifier(sqlHelper, values["RDB$FOREIGN_KEY"].DbValueToString()),
                ExpressionSource = values["RDB$EXPRESSION_SOURCE"].DbValueToString(),
                SystemFlag = (SystemFlagType)values["RDB$SYSTEM_FLAG"].DbValueToInt32().GetValueOrDefault()
            };
        result.Segments = indexSegments[result.IndexName].OrderBy(s => s.FieldPosition).ToArray();
        return result;
    }
}
