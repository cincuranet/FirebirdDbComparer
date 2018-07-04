using System.Collections.Generic;

using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.Primitives;

namespace FirebirdDbComparer.Interfaces
{
    public interface IMetadataPackages : IDatabaseObject
    {
        IDictionary<Identifier, Package> PackagesByName { get; }
    }
}
