using FirebirdDbComparer.DatabaseObjects;

namespace FirebirdDbComparer.Interfaces
{
    public interface IHasDefaultSource
    {
        DatabaseStringOrdinal DefaultSource { get; }
    }
}
