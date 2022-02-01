using FirebirdDbComparer.Common.Equatable;

namespace FirebirdDbComparer.DatabaseObjects.EquatableKeys;

public sealed class CollationKey : BasicElement<CollationKey>
{
    private static readonly EquatableProperty<CollationKey>[] s_EquatableProperties =
    {
            new EquatableProperty<CollationKey>(x => x.CollationId, nameof(CollationId)),
            new EquatableProperty<CollationKey>(x => x.CharacterSetId, nameof(CharacterSetId))
        };

    public CollationKey(int characterSetId, int collationId)
    {
        CollationId = collationId;
        CharacterSetId = characterSetId;
    }

    public int CollationId { get; }
    public int CharacterSetId { get; }

    protected override CollationKey Self => this;

    protected override EquatableProperty<CollationKey>[] EquatableProperties => s_EquatableProperties;
}
