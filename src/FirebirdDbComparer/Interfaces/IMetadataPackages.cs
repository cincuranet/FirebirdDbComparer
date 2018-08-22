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
        IEnumerable<CommandGroup> DropPackages(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> AlterPackagesHeaders(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> AlterPackagesBodies(IMetadata other, IComparerContext context);
    }
}
