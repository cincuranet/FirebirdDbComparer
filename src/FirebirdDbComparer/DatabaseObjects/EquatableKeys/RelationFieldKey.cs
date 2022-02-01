using System.Diagnostics;

using FirebirdDbComparer.Common.Equatable;

namespace FirebirdDbComparer.DatabaseObjects.EquatableKeys;

[DebuggerDisplay("{RelationName}.{FieldName}")]
public sealed class RelationFieldKey : BasicElement<RelationFieldKey>
{
    private static readonly EquatableProperty<RelationFieldKey>[] s_EquatableProperties =
    {
            new EquatableProperty<RelationFieldKey>(x => x.RelationName, nameof(RelationName)),
            new EquatableProperty<RelationFieldKey>(x => x.FieldName, nameof(FieldName))
        };

    public RelationFieldKey(Identifier relationName, Identifier fieldName)
    {
        RelationName = relationName;
        FieldName = fieldName;
    }

    public Identifier RelationName { get; }
    public Identifier FieldName { get; }

    protected override RelationFieldKey Self => this;

    protected override EquatableProperty<RelationFieldKey>[] EquatableProperties => s_EquatableProperties;
}
