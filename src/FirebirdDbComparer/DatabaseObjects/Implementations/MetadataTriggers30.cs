using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataTriggers30 : MetadataTriggers25
{
    public MetadataTriggers30(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    protected override string CommandText => @"
select trim(T.RDB$TRIGGER_NAME) as RDB$TRIGGER_NAME,
       trim(T.RDB$RELATION_NAME) as RDB$RELATION_NAME,
       T.RDB$TRIGGER_SEQUENCE,
       T.RDB$TRIGGER_TYPE,
       T.RDB$TRIGGER_SOURCE,
       T.RDB$DESCRIPTION,
       T.RDB$TRIGGER_INACTIVE,
       T.RDB$SYSTEM_FLAG,
       trim(T.RDB$ENGINE_NAME) as RDB$ENGINE_NAME,
       trim(T.RDB$ENTRYPOINT) as RDB$ENTRYPOINT
  from RDB$TRIGGERS T";
}
