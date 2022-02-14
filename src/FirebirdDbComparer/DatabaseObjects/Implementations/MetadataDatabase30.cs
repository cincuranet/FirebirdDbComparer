using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataDatabase30 : MetadataDatabase25
{
    public MetadataDatabase30(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    public int? Linger { get; private set; }

    protected override string CommandText => @"
select D.RDB$DESCRIPTION,
       trim(D.RDB$CHARACTER_SET_NAME) as RDB$CHARACTER_SET_NAME,
       M.MON$SQL_DIALECT,
       M.MON$ODS_MAJOR,
       M.MON$ODS_MINOR,
       D.RDB$LINGER
  from RDB$DATABASE D
       cross join MON$DATABASE M";

    protected override void Initialize(IDictionary<string, object> values)
    {
        base.Initialize(values);

        Linger = values["RDB$LINGER"].DbValueToInt32();
    }
}
