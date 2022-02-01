using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataCharacterSets30 : MetadataCharacterSets25
{
    public MetadataCharacterSets30(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    protected override string CommandText => @"
select trim(CS.RDB$CHARACTER_SET_NAME) as RDB$CHARACTER_SET_NAME,
       CS.RDB$NUMBER_OF_CHARACTERS,
       trim(CS.RDB$DEFAULT_COLLATE_NAME) as RDB$DEFAULT_COLLATE_NAME,
       CS.RDB$CHARACTER_SET_ID,
       CS.RDB$SYSTEM_FLAG,
       CS.RDB$DESCRIPTION,
       CS.RDB$BYTES_PER_CHARACTER,
       trim(CS.RDB$OWNER_NAME) as RDB$OWNER_NAME
  from RDB$CHARACTER_SETS CS";
}
