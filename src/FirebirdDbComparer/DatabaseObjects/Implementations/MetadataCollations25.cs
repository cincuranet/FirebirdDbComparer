using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataCollations25 : DatabaseObject, IMetadataCollations, ISupportsComment
{
    private IDictionary<CollationKey, Collation> m_CollationsByKey;
    private IDictionary<Identifier, Collation> m_CollationsByName;
    private IDictionary<int, IList<Collation>> m_CollationsByCharacterSetId;

    public MetadataCollations25(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    public IDictionary<CollationKey, Collation> CollationsByKey => m_CollationsByKey;

    public IDictionary<Identifier, Collation> CollationsByName => m_CollationsByName;

    public IDictionary<int, IList<Collation>> CollationsByCharacterSetId => m_CollationsByCharacterSetId;

    protected virtual string CommandText => @"
select trim(C.RDB$COLLATION_NAME) as RDB$COLLATION_NAME,
       C.RDB$COLLATION_ID,
       C.RDB$CHARACTER_SET_ID,
       C.RDB$COLLATION_ATTRIBUTES,
       C.RDB$SYSTEM_FLAG,
       C.RDB$DESCRIPTION,
       C.RDB$FUNCTION_NAME,
       trim(C.RDB$BASE_COLLATION_NAME) as RDB$BASE_COLLATION_NAME,
       C.RDB$SPECIFIC_ATTRIBUTES
  from RDB$COLLATIONS C";

    public override void Initialize()
    {
        var collations = Execute(CommandText)
            .Select(o => Collation.CreateFrom(SqlHelper, o))
            .ToArray();
        m_CollationsByKey = collations.ToDictionary(x => new CollationKey(x.CharacterSetId, x.CollationId));
        m_CollationsByName = collations.ToDictionary(x => x.CollationName);
        m_CollationsByCharacterSetId = m_CollationsByKey.Values.ToMultiDictionary(x => x.CharacterSetId);
    }

    public override void FinishInitialization()
    {
        foreach (var collation in CollationsByName.Values)
        {
            collation.CharacterSet = Metadata.MetadataCharacterSets.CharacterSetsById[collation.CharacterSetId];
        }
    }

    IEnumerable<CommandGroup> ISupportsComment.Handle(IMetadata other, IComparerContext context)
    {
        var result = new CommandGroup().Append(HandleComment(CollationsByName, other.MetadataCollations.CollationsByName, x => x.CollationName, "COLLATION", x => new[] { x.CollationName }, context));
        if (!result.IsEmpty)
        {
            yield return result;
        }
    }

    public IEnumerable<CommandGroup> CreateCollations(IMetadata other, IComparerContext context)
    {
        return FilterSystemFlagUser(CollationsByName.Values)
            .Where(c => !other.MetadataCollations.CollationsByName.ContainsKey(c.CollationName))
            .Select(c => new CommandGroup().Append(c.Create(Metadata, other, context)));
    }

    public IEnumerable<CommandGroup> DropCollations(IMetadata other, IComparerContext context)
    {
        return FilterSystemFlagUser(other.MetadataCollations.CollationsByName.Values)
            .Where(c => !CollationsByName.ContainsKey(c.CollationName))
            .Select(c => new CommandGroup().Append(c.Drop(Metadata, other, context)));
    }
}
