using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataRoles40 : MetadataRoles30
{
    public MetadataRoles40(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    protected override string CommandText => @"
select trim(R.RDB$ROLE_NAME) as RDB$ROLE_NAME,
       trim(R.RDB$OWNER_NAME) as RDB$OWNER_NAME,
       R.RDB$DESCRIPTION,
       R.RDB$SYSTEM_FLAG,
       R.RDB$SYSTEM_PRIVILEGES
  from RDB$ROLES R";
}
