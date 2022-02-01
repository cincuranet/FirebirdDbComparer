using FirebirdDbComparer.Common.Equatable;

namespace FirebirdDbComparer.DatabaseObjects.EquatableKeys;

public sealed class FunctionArgumentKey : BasicElement<FunctionArgumentKey>
{
    private static readonly EquatableProperty<FunctionArgumentKey>[] s_EquatableProperties =
    {
            new EquatableProperty<FunctionArgumentKey>(x => x.FunctionNameKey, nameof(FunctionNameKey)),
            new EquatableProperty<FunctionArgumentKey>(x => x.ArgumentPosition, nameof(ArgumentPosition)),
            new EquatableProperty<FunctionArgumentKey>(x => x.ArgumentName, nameof(ArgumentName))
        };

    public FunctionArgumentKey(Identifier functionNameKey, int argumentPosition, Identifier argumentName)
    {
        FunctionNameKey = functionNameKey;
        ArgumentPosition = argumentPosition;
        ArgumentName = argumentName;
    }

    public Identifier FunctionNameKey { get; }
    public int ArgumentPosition { get; }
    public Identifier ArgumentName { get; }

    protected override FunctionArgumentKey Self => this;

    protected override EquatableProperty<FunctionArgumentKey>[] EquatableProperties => s_EquatableProperties;
}
