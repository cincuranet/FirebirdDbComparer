using System.Collections.Generic;

using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Interfaces;

public interface IMetadataUserPrivileges : IDatabaseObject
{
    IList<UserPrivilege> UserPrivileges { get; }
    IEnumerable<CommandGroup> HandleUserPrivileges(IMetadata other, IComparerContext context);
}
