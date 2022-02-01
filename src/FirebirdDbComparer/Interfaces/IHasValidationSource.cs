using FirebirdDbComparer.DatabaseObjects;

namespace FirebirdDbComparer.Interfaces;

public interface IHasValidationSource
{
    DatabaseStringOrdinal ValidationSource { get; }
}
