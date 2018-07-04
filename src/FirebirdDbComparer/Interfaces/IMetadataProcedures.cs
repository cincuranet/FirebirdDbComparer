using System.Collections.Generic;
using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.Elements;
using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Interfaces
{
    public interface IMetadataProcedures : IDatabaseObject
    {
        IDictionary<ProcedureParameterKey, ProcedureParameter> ProcedureParameters { get; }
        IDictionary<int, Procedure> ProceduresById { get; }
        IDictionary<Identifier, Procedure> ProceduresByName { get; }
        IEnumerable<CommandGroup> CreateEmptyProcedures(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> AlterProceduresToFullBody(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> AlterProceduresToEmptyBodyForAlteringOrDropping(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> DropProcedures(IMetadata other, IComparerContext context);
    }
}
