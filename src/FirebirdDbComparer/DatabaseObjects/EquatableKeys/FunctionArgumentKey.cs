using FirebirdDbComparer.Common.Equatable;

namespace FirebirdDbComparer.DatabaseObjects.EquatableKeys
{
    public sealed class FunctionArgumentKey : BasicElement<FunctionArgumentKey>
    {
        private static readonly EquatableProperty<FunctionArgumentKey>[] s_EquatableProperties =
        {
            new EquatableProperty<FunctionArgumentKey>(x => x.FunctionName, nameof(FunctionName)),
            new EquatableProperty<FunctionArgumentKey>(x => x.ArgumentPosition, nameof(ArgumentPosition))
        };

        public FunctionArgumentKey(Identifier functionName, int argumentPosition)
        {
            FunctionName = functionName;
            ArgumentPosition = argumentPosition;
        }

        public Identifier FunctionName { get; }
        public int ArgumentPosition { get; }

        protected override FunctionArgumentKey Self => this;

        protected override EquatableProperty<FunctionArgumentKey>[] EquatableProperties => s_EquatableProperties;
    }
}
