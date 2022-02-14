using System;
using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataPackages40 : MetadataPackages30
{
    public MetadataPackages40(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    protected override string CommandText => @"
select trim(P.RDB$PACKAGE_NAME) as RDB$PACKAGE_NAME,
       P.RDB$PACKAGE_HEADER_SOURCE,
       P.RDB$PACKAGE_BODY_SOURCE,
       P.RDB$VALID_BODY_FLAG,
       trim(P.RDB$OWNER_NAME) as RDB$OWNER_NAME,
       P.RDB$SYSTEM_FLAG,
       P.RDB$DESCRIPTION,
       P.RDB$SQL_SECURITY
  from RDB$PACKAGES P";
}
