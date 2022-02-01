using System.Collections.Generic;
using System.Linq;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataProcedures30 : MetadataProcedures25, ISupportsComment
{
    private IDictionary<Identifier, Procedure> m_NonPackageProceduresByName;

    public MetadataProcedures30(IMetadata metadata, ISqlHelper sqlHelper)
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
       P.RDB$PRIVATE_FLAG
  from RDB$PROCEDURES P";

    protected override string ProcedureParameterCommandText => @"
select trim(PP.RDB$PARAMETER_NAME) as RDB$PARAMETER_NAME,
       trim(PP.RDB$PROCEDURE_NAME) as RDB$PROCEDURE_NAME,
       PP.RDB$PARAMETER_NUMBER,
       PP.RDB$PARAMETER_TYPE,
       trim(PP.RDB$FIELD_SOURCE) as RDB$FIELD_SOURCE,
       PP.RDB$DESCRIPTION,
       PP.RDB$SYSTEM_FLAG,
       PP.RDB$DEFAULT_SOURCE,
       PP.RDB$COLLATION_ID,
       iif(coalesce(PP.RDB$NULL_FLAG, 0) = 0, 0, 1) as RDB$NULL_FLAG,
       PP.RDB$PARAMETER_MECHANISM,
       trim(PP.RDB$FIELD_NAME) as RDB$FIELD_NAME,
       trim(PP.RDB$RELATION_NAME) as RDB$RELATION_NAME,
       trim(PP.RDB$PACKAGE_NAME) as RDB$PACKAGE_NAME
  from RDB$PROCEDURE_PARAMETERS PP";

    public override IDictionary<Identifier, Procedure> NonPackageProceduresByName => m_NonPackageProceduresByName;

    public override void FinishInitialization()
    {
        base.FinishInitialization();

        m_NonPackageProceduresByName = ProceduresById.Values
            .Where(x => x.PackageName == null)
            .ToDictionary(x => x.ProcedureNameKey, x => x);

        foreach (var procedureParameter in ProcedureParameters.Values)
        {
            if (procedureParameter.PackageName != null)
            {
                procedureParameter.Package =
                    Metadata
                        .MetadataPackages
                        .PackagesByName[procedureParameter.PackageName];
            }
        }

        foreach (var procedure in ProceduresById.Values)
        {
            if (procedure.PackageName != null)
            {
                procedure.Package =
                    Metadata
                        .MetadataPackages
                        .PackagesByName[procedure.PackageName];
            }
        }
    }

    protected override IEnumerable<Procedure> FilterNewProcedures(IMetadata other)
    {
        return FilterSystemFlagUser(NonPackageProceduresByName.Values)
            .Where(p => !other.MetadataProcedures.NonPackageProceduresByName.ContainsKey(p.ProcedureNameKey));
    }

    protected override IEnumerable<Procedure> FilterProceduresToBeDropped(IMetadata other)
    {
        return FilterSystemFlagUser(other.MetadataProcedures.NonPackageProceduresByName.Values)
            .Where(p => !NonPackageProceduresByName.ContainsKey(p.ProcedureNameKey));
    }

    protected override IEnumerable<Procedure> FilterProceduresToBeAltered(IMetadata other)
    {
        return FilterSystemFlagUser(NonPackageProceduresByName.Values)
            .Where(p => other.MetadataProcedures.NonPackageProceduresByName.TryGetValue(p.ProcedureNameKey, out var otherProcedure) && otherProcedure != p);
    }
}
