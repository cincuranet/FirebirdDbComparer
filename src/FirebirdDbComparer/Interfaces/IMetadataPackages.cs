using System.Collections.Generic;

using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Interfaces
{
    public interface IMetadataPackages : IDatabaseObject
    {
        IDictionary<Identifier, Package> PackagesByName { get; }
        IEnumerable<CommandGroup> CreateNewPackagesHeaders(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> CreateNewPackagesBodies(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> DropFullPackages(IMetadata other, IComparerContext context);
    }
}
