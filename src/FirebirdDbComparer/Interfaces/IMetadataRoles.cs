using System.Collections.Generic;
using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Interfaces
{
    public interface IMetadataRoles : IDatabaseObject
    {
        IDictionary<Identifier, Role> Roles { get; }
        IEnumerable<CommandGroup> CreateRoles(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> DropRoles(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> AlterRoles(IMetadata other, IComparerContext context);
    }
}
