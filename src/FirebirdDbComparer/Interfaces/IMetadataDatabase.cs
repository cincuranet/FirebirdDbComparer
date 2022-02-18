using System.Collections.Generic;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Interfaces;

public interface IMetadataDatabase : IDatabaseObject
{
    Database Database { get; }
    CommandGroup ProcessDatabase(IMetadata other, IComparerContext context);
}
