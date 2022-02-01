using FirebirdDbComparer.Common.Equatable;

namespace FirebirdDbComparer.DatabaseObjects.EquatableKeys;

public sealed class ProcedureParameterKey : BasicElement<ProcedureParameterKey>
{
    private static readonly EquatableProperty<ProcedureParameterKey>[] s_EquatableProperties =
    {
            new EquatableProperty<ProcedureParameterKey>(x => x.ProcedureNameKey, nameof(ProcedureNameKey)),
            new EquatableProperty<ProcedureParameterKey>(x => x.ParameterName, nameof(ParameterName))
        };

    public ProcedureParameterKey(Identifier procedureNameKey, Identifier parameterName)
    {
        ProcedureNameKey = procedureNameKey;
        ParameterName = parameterName;
    }

    public Identifier ProcedureNameKey { get; }
    public Identifier ParameterName { get; }

    protected override ProcedureParameterKey Self => this;

    protected override EquatableProperty<ProcedureParameterKey>[] EquatableProperties => s_EquatableProperties;
}
