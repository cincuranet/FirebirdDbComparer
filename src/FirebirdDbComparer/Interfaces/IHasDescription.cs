using FirebirdDbComparer.DatabaseObjects;

namespace FirebirdDbComparer.Interfaces
{
    public interface IHasDescription
    {
        DatabaseStringOrdinal Description { get; }
    }
}
