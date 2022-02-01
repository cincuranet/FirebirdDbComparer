using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataFields30 : MetadataFields25
{
    public MetadataFields30(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    // RDB$FIELD_SUB_TYPE: weird discrepancy in some databases
    // RDB$FIELD_SCALE: weird discrepancy in some databases
    // RDB$FIELD_PRECISION: CORE-5550
    protected override string CommandText => @"
select trim(F.RDB$FIELD_NAME) as RDB$FIELD_NAME,
       trim(F.RDB$DESCRIPTION) as RDB$DESCRIPTION,
       trim(F.RDB$COMPUTED_SOURCE) as RDB$COMPUTED_SOURCE,
       trim(F.RDB$DEFAULT_SOURCE) as RDB$DEFAULT_SOURCE,
       trim(F.RDB$VALIDATION_SOURCE) as RDB$VALIDATION_SOURCE,
       F.RDB$FIELD_TYPE,
       coalesce(F.RDB$FIELD_SUB_TYPE, 0) as RDB$FIELD_SUB_TYPE,
       F.RDB$FIELD_LENGTH,
       coalesce(F.RDB$FIELD_SCALE, 0) as RDB$FIELD_SCALE,
       coalesce(F.RDB$FIELD_PRECISION, 0) as RDB$FIELD_PRECISION,
       F.RDB$SEGMENT_LENGTH,
       F.RDB$CHARACTER_LENGTH,
       F.RDB$CHARACTER_SET_ID,
       F.RDB$COLLATION_ID,
       iif(coalesce(F.RDB$NULL_FLAG, 0) = 0, 0, 1) as RDB$NULL_FLAG,
       F.RDB$SYSTEM_FLAG,
       trim(F.RDB$OWNER_NAME) as RDB$OWNER_NAME
  from RDB$FIELDS F";
}
