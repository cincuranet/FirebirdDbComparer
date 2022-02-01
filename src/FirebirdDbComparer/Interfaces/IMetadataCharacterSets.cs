using System.Collections.Generic;
using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Interfaces;

public interface IMetadataCharacterSets : IDatabaseObject
{
    IDictionary<int, CharacterSet> CharacterSetsById { get; }
    IDictionary<Identifier, CharacterSet> CharacterSetsByName { get; }
    IEnumerable<CommandGroup> AlterCharacterSets(IMetadata other, IComparerContext context);
}
