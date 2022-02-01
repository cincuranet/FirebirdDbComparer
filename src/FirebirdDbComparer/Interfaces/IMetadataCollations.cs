using System.Collections.Generic;
using System.Linq;
using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Interfaces;

public interface IMetadataCollations : IDatabaseObject
{
    IDictionary<CollationKey, Collation> CollationsByKey { get; }
    IDictionary<Identifier, Collation> CollationsByName { get; }
    IDictionary<int, IList<Collation>> CollationsByCharacterSetId { get; }
    IEnumerable<CommandGroup> CreateCollations(IMetadata other, IComparerContext context);
    IEnumerable<CommandGroup> DropCollations(IMetadata other, IComparerContext context);
}
