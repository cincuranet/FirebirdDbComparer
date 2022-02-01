using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Exceptions;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Primitives;

[DebuggerDisplay("{RelationName}.{ConstraintName}")]
public sealed class RelationConstraint : Primitive<RelationConstraint>
{
    private sealed class CheckConstraintEqualityComparer : PropertiesEqualityComparerBase<RelationConstraint>
    {
        private static readonly EquatableProperty<RelationConstraint>[] s_EquatableProperties =
        {
                new EquatableProperty<RelationConstraint>(x => x.RelationName, nameof(RelationName)),
                new EquatableProperty<RelationConstraint>(x => x.Triggers.Select(y => y.TriggerSource), nameof(Triggers))
            };

        public CheckConstraintEqualityComparer()
            : base(s_EquatableProperties)
        { }
    }

    private static readonly EquatableProperty<RelationConstraint>[] s_EquatableProperties =
    {
            // keep RelationConstraintType and RelationName first to have possible negative match first
            new EquatableProperty<RelationConstraint>(x => x.RelationConstraintType, nameof(RelationConstraintType)),
            new EquatableProperty<RelationConstraint>(x => x.RelationName, nameof(RelationName)),
            new EquatableProperty<RelationConstraint>(x => x._EqualityConstraintName, nameof(ConstraintName)),
            new EquatableProperty<RelationConstraint>(x => x._EqualityIndex, nameof(Index)),
            new EquatableProperty<RelationConstraint>(x => x.FieldName, nameof(FieldName))
        };

    public RelationConstraint(ISqlHelper sqlHelper)
        : base(sqlHelper)
    { }

    public static IEqualityComparer<RelationConstraint> CheckConstraintComparer { get; } = new CheckConstraintEqualityComparer();

    internal Identifier _EqualityConstraintName => SqlHelper.IsImplicitIntegrityConstraintName(ConstraintName) ? null : ConstraintName;
    public Identifier ConstraintName { get; private set; }
    public RelationConstraintType RelationConstraintType { get; private set; }
    public Identifier RelationName { get; private set; }
    public Relation Relation { get; set; }
    public Identifier IndexName { get; private set; }
    internal Index _EqualityIndex => SqlHelper.IsImplicitIntegrityConstraintName(ConstraintName) ? null : Index;
    public Index Index { get; set; }
    public Identifier FieldName { get; set; }
    public IList<Trigger> Triggers { get; set; }

    protected override RelationConstraint Self => this;

    protected override EquatableProperty<RelationConstraint>[] EquatableProperties => s_EquatableProperties;

    protected override IEnumerable<Command> OnCreate(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
    {
        var command = new Command();
        command.Append($"ALTER TABLE {RelationName.AsSqlIndentifier()}");
        if (!SqlHelper.IsImplicitIntegrityConstraintName(ConstraintName))
        {
            command.Append($" ADD CONSTRAINT {ConstraintName.AsSqlIndentifier()}");
        }
        else
        {
            command.Append(" ADD");
        }

        switch (RelationConstraintType)
        {
            case RelationConstraintType.Check:
                command.Append($" {Triggers[0].TriggerSource}");
                break;

            case RelationConstraintType.NotNull:
                break;

            case RelationConstraintType.ForeignKey:
            case RelationConstraintType.PrimaryKey:
            case RelationConstraintType.Unique:
                {
                    var fields =
                        Index
                            .Segments
                            .OrderBy(s => s.FieldPosition)
                            .Select(s => s.FieldName);

                    command.Append($" {RelationConstraintType.ToDescription()} ({string.Join(", ", fields)})");

                    if (RelationConstraintType == RelationConstraintType.ForeignKey)
                    {
                        var referenceConstraint = sourceMetadata.MetadataConstraints.ReferenceConstraintsByName[ConstraintName];
                        var referenceRelationConstraint = referenceConstraint.RelationConstraintUq;
                        var primaryKeyFields =
                            referenceRelationConstraint
                                .Index
                                .Segments
                                .OrderBy(s => s.FieldPosition)
                                .Select(s => s.FieldName);

                        command
                            .AppendLine()
                            .Append($"  REFERENCES {referenceRelationConstraint.RelationName} ({string.Join(", ", primaryKeyFields)})");
                        if (referenceConstraint.UpdateRule != ConstraintRule.Restrict)
                        {
                            command
                                .AppendLine()
                                .Append($"  ON UPDATE {referenceConstraint.UpdateRule.ToDescription()}");
                        }
                        if (referenceConstraint.DeleteRule != ConstraintRule.Restrict)
                        {
                            command
                                .AppendLine()
                                .Append($"  ON DELETE {referenceConstraint.DeleteRule.ToDescription()}");
                        }
                        if (referenceConstraint.UpdateRule != ConstraintRule.Restrict || referenceConstraint.DeleteRule != ConstraintRule.Restrict)
                        {
                            command.AppendLine();
                        }
                    }

                    command.Append(AddConstraintUsingIndex(sourceMetadata, targetMetadata, context));
                    break;
                }

            default:
                throw new ArgumentOutOfRangeException($"Unknown relation constraint type: {RelationConstraintType}.");
        }

        yield return command;
    }

    protected override IEnumerable<Command> OnDrop(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
    {
        yield return new Command()
            .Append($"ALTER TABLE {RelationName.AsSqlIndentifier()} DROP CONSTRAINT {ConstraintName.AsSqlIndentifier()}");
    }

    protected override IEnumerable<Command> OnAlter(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
    {
        throw new DeadCodePathException();
    }

    protected override Identifier OnPrimitiveTypeKeyObjectName() => ConstraintName;

    internal static RelationConstraint CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values)
    {
        var constraintType = values["RDB$CONSTRAINT_TYPE"].DbValueToString();

        var result =
            new RelationConstraint(sqlHelper)
            {
                ConstraintName = new Identifier(sqlHelper, values["RDB$CONSTRAINT_NAME"].DbValueToString()),
                RelationConstraintType = ConvertFrom(constraintType),
                RelationName = new Identifier(sqlHelper, values["RDB$RELATION_NAME"].DbValueToString()),
                IndexName = new Identifier(sqlHelper, values["RDB$INDEX_NAME"].DbValueToString())
            };
        return result;
    }

    private static RelationConstraintType ConvertFrom(string constraintType)
    {
        RelationConstraintType result;
        switch (constraintType)
        {
            case "CHECK":
                result = RelationConstraintType.Check;
                break;
            case "FOREIGN KEY":
                result = RelationConstraintType.ForeignKey;
                break;
            case "NOT NULL":
                result = RelationConstraintType.NotNull;
                break;
            case "PRIMARY KEY":
                result = RelationConstraintType.PrimaryKey;
                break;
            case "UNIQUE":
                result = RelationConstraintType.Unique;
                break;
            default:
                throw new ArgumentOutOfRangeException($"Unknown constraint type: {constraintType}.");
        }
        return result;
    }

    private string AddConstraintUsingIndex(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
    {
        var builder = new StringBuilder();
        if (RelationConstraintType == RelationConstraintType.PrimaryKey || RelationConstraintType == RelationConstraintType.ForeignKey || RelationConstraintType == RelationConstraintType.Unique)
        {
            if (ConstraintName != IndexName && !SqlHelper.HasSystemPrefix(IndexName) || Index.Descending)
            {
                builder
                    .AppendLine()
                    .Append($"  USING {(Index.Descending ? "DESCENDING" : "ASCENDING")} INDEX {IndexName.AsSqlIndentifier()}");
            }
        }
        return builder.ToString();
    }
}
