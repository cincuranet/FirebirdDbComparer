using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataCharacterSets25 : DatabaseObject, IMetadataCharacterSets, ISupportsComment
{
    private IDictionary<int, CharacterSet> m_CharacterSetsById;
    private IDictionary<Identifier, CharacterSet> m_CharacterSetsByName;

    public MetadataCharacterSets25(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    public IDictionary<int, CharacterSet> CharacterSetsById => m_CharacterSetsById;

    public IDictionary<Identifier, CharacterSet> CharacterSetsByName => m_CharacterSetsByName;

    protected virtual string CommandText => @"
select trim(CS.RDB$CHARACTER_SET_NAME) as RDB$CHARACTER_SET_NAME,
       CS.RDB$NUMBER_OF_CHARACTERS,
       trim(CS.RDB$DEFAULT_COLLATE_NAME) as RDB$DEFAULT_COLLATE_NAME,
       CS.RDB$CHARACTER_SET_ID,
       CS.RDB$SYSTEM_FLAG,
       CS.RDB$DESCRIPTION,
       CS.RDB$BYTES_PER_CHARACTER
from RDB$CHARACTER_SETS CS";

    public override void Initialize()
    {
        var characterSets =
            Execute(CommandText)
                .Select(o => CharacterSet.CreateFrom(SqlHelper, o))
                .ToArray();
        m_CharacterSetsById = characterSets.ToDictionary(x => x.CharacterSetId);
        m_CharacterSetsByName = characterSets.ToDictionary(x => x.CharacterSetName);
    }

    public override void FinishInitialization()
    {
        foreach (var characterSet in m_CharacterSetsByName.Values)
        {
            characterSet.Collations =
                Metadata
                    .MetadataCollations
                    .CollationsByCharacterSetId[characterSet.CharacterSetId]
                    .ToArray();
            characterSet.DefaultCollation =
                Metadata
                    .MetadataCollations
                    .CollationsByName[characterSet.DefaultCollateName];
        }
    }

    IEnumerable<CommandGroup> ISupportsComment.Handle(IMetadata other, IComparerContext context)
    {
        var result = new CommandGroup().Append(HandleComment(CharacterSetsByName, other.MetadataCharacterSets.CharacterSetsByName, x => x.CharacterSetName, "CHARACTER SET", x => new[] { x.CharacterSetName }, context));
        if (!result.IsEmpty)
        {
            yield return result;
        }
    }

    public IEnumerable<CommandGroup> AlterCharacterSets(IMetadata other, IComparerContext context)
    {
        return CharacterSetsById.Values
            .Where(ch => other.MetadataCharacterSets.CharacterSetsById.TryGetValue(ch.CharacterSetId, out var otherCharacterSet) && ch != otherCharacterSet)
            .Select(ch => new CommandGroup().Append(ch.Alter(Metadata, other, context)))
            .Where(x => !x.IsEmpty);
    }
}
