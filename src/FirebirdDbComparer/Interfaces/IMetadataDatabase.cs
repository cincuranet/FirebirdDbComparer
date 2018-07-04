using System.Collections.Generic;
using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Interfaces
{
    public interface IMetadataDatabase : IDatabaseObject
    {
        DatabaseStringOrdinal Description { get; }
        Identifier CharacterSetName { get; }
        CharacterSet CharacterSet { get; }
        int Dialect { get; }
        short OdsMajor { get; }
        short OdsMinor { get; }
        IEnumerable<CommandGroup> HandleDatabase(IMetadata other, IComparerContext context);
    }
}
