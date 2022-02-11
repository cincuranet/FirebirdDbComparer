using System;

using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataRelations40 : MetadataRelations30
{
    public MetadataRelations40(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    protected override string RelationCommandText => @"
select R.RDB$RELATION_ID,
       R.RDB$RELATION_TYPE,
       trim(R.RDB$RELATION_NAME) as RDB$RELATION_NAME,
       R.RDB$DESCRIPTION,
       R.RDB$VIEW_SOURCE,
       R.RDB$EXTERNAL_FILE,
       R.RDB$EXTERNAL_DESCRIPTION,
       trim(R.RDB$OWNER_NAME) as RDB$OWNER_NAME,
       R.RDB$SYSTEM_FLAG,
       R.RDB$SQL_SECURITY
  from RDB$RELATIONS R";
}
