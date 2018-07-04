using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Implementations
{
    public class MetadataExceptions30 : MetadataExceptions25
    {
        public MetadataExceptions30(IMetadata metadata, ISqlHelper sqlHelper)
            : base(metadata, sqlHelper)
        { }

        protected override string CommandText => @"
select trim(E.RDB$EXCEPTION_NAME) as RDB$EXCEPTION_NAME,
       E.RDB$EXCEPTION_NUMBER,
       E.RDB$MESSAGE,
       E.RDB$DESCRIPTION,
       E.RDB$SYSTEM_FLAG,
       trim(E.RDB$OWNER_NAME) as RDB$OWNER_NAME
  from RDB$EXCEPTIONS E";
    }
}
