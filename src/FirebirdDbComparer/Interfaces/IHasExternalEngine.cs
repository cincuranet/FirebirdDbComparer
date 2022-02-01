using FirebirdDbComparer.DatabaseObjects;

namespace FirebirdDbComparer.Interfaces;

public interface IHasExternalEngine
{
    Identifier EngineName { get; }
    DatabaseStringOrdinal EntryPoint { get; }
}
