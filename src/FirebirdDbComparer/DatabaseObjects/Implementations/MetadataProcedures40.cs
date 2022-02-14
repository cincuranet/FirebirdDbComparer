using System.Collections.Generic;
using System.Linq;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataProcedures40 : MetadataProcedures30
{
    public MetadataProcedures40(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    protected override string ProcedureCommandText => @"
select trim(P.RDB$PROCEDURE_NAME) as RDB$PROCEDURE_NAME,
       P.RDB$PROCEDURE_ID,
       P.RDB$PROCEDURE_INPUTS,
       P.RDB$PROCEDURE_OUTPUTS,
       P.RDB$DESCRIPTION,
       P.RDB$PROCEDURE_SOURCE,
       trim(P.RDB$OWNER_NAME) as RDB$OWNER_NAME,
       P.RDB$PROCEDURE_TYPE,
       P.RDB$SYSTEM_FLAG,
       trim(P.RDB$ENGINE_NAME) as RDB$ENGINE_NAME,
       trim(P.RDB$ENTRYPOINT) as RDB$ENTRYPOINT,
       trim(P.RDB$PACKAGE_NAME) as RDB$PACKAGE_NAME,
       P.RDB$PRIVATE_FLAG,
       P.RDB$SQL_SECURITY
  from RDB$PROCEDURES P";
}
