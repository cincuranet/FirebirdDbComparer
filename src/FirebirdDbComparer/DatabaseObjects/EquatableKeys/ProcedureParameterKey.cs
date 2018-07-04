using FirebirdDbComparer.Common.Equatable;

namespace FirebirdDbComparer.DatabaseObjects.EquatableKeys
{
    public sealed class ProcedureParameterKey : BasicElement<ProcedureParameterKey>
    {
        private static readonly EquatableProperty<ProcedureParameterKey>[] s_EquatableProperties =
        {
            new EquatableProperty<ProcedureParameterKey>(x => x.ProcedureName, nameof(ProcedureName)),
            new EquatableProperty<ProcedureParameterKey>(x => x.ParameterName, nameof(ParameterName))
        };

        public ProcedureParameterKey(Identifier procedureName, Identifier parameterName)
        {
            ProcedureName = procedureName;
            ParameterName = parameterName;
        }

        public Identifier ProcedureName { get; }
        public Identifier ParameterName { get; }

        protected override ProcedureParameterKey Self => this;

        protected override EquatableProperty<ProcedureParameterKey>[] EquatableProperties => s_EquatableProperties;
    }
}
