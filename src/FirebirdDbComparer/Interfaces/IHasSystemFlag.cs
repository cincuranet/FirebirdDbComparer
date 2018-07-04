using FirebirdDbComparer.DatabaseObjects;

namespace FirebirdDbComparer.Interfaces
{
    public interface IHasSystemFlag
    {
        SystemFlagType SystemFlag { get; }
    }
}
