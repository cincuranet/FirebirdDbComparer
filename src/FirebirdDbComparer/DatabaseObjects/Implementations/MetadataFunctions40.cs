using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Exceptions;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataFunctions40 : MetadataFunctions30
{
    public MetadataFunctions40(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    protected override string FunctionCommandText => @"
select trim(F.RDB$FUNCTION_NAME) as RDB$FUNCTION_NAME,
       F.RDB$DESCRIPTION,
       F.RDB$MODULE_NAME,
       trim(F.RDB$ENTRYPOINT) as RDB$ENTRYPOINT,
       F.RDB$RETURN_ARGUMENT,
       F.RDB$SYSTEM_FLAG,
       trim(F.RDB$ENGINE_NAME) as RDB$ENGINE_NAME,
       trim(F.RDB$PACKAGE_NAME) RDB$PACKAGE_NAME,
       F.RDB$PRIVATE_FLAG,
       F.RDB$FUNCTION_SOURCE,
       F.RDB$OWNER_NAME,
       F.RDB$LEGACY_FLAG,
       F.RDB$DETERMINISTIC_FLAG,
       F.RDB$SQL_SECURITY
  from RDB$FUNCTIONS F";
}
