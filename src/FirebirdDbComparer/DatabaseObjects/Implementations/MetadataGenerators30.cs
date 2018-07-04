using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Implementations
{
    public class MetadataGenerators30 : MetadataGenerators25
    {
        public MetadataGenerators30(IMetadata metadata, ISqlHelper sqlHelper)
            : base(metadata, sqlHelper)
        { }

        protected override string CommandText => @"
select trim(G.RDB$GENERATOR_NAME) as RDB$GENERATOR_NAME,
       G.RDB$GENERATOR_ID,
       G.RDB$SYSTEM_FLAG,
       G.RDB$DESCRIPTION,
       G.RDB$OWNER_NAME,
       G.RDB$INITIAL_VALUE,
       G.RDB$GENERATOR_INCREMENT
  from RDB$GENERATORS G";
    }
}
