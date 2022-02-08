using System;
using System.Linq;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataUserPrivileges40 : MetadataUserPrivileges30
{
    public MetadataUserPrivileges40(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    protected override void AddDefault(UserPrivilege privilege, Command command)
    {
        if (privilege.FieldName == "D")
        {
            command.Append(" DEFAULT");
        }
    }
}
