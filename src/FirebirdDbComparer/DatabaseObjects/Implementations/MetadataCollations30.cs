using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Implementations
{
    public class MetadataCollations30 : MetadataCollations25
    {
        public MetadataCollations30(IMetadata metadata, ISqlHelper sqlHelper)
            : base(metadata, sqlHelper)
        { }

        protected override string CommandText => @"
select trim(C.RDB$COLLATION_NAME) as RDB$COLLATION_NAME,
       C.RDB$COLLATION_ID,
       C.RDB$CHARACTER_SET_ID,
       C.RDB$COLLATION_ATTRIBUTES,
       C.RDB$SYSTEM_FLAG,
       C.RDB$DESCRIPTION,
       C.RDB$FUNCTION_NAME,
       trim(C.RDB$BASE_COLLATION_NAME) as RDB$BASE_COLLATION_NAME,
       C.RDB$SPECIFIC_ATTRIBUTES,
       trim(C.RDB$OWNER_NAME) as RDB$OWNER_NAME
  from RDB$COLLATIONS C";
    }
}
