using FirebirdDbComparer.DatabaseObjects;

namespace FirebirdDbComparer.Interfaces
{
    public interface IHasPackage
    {
        Identifier PackageName { get; }
    }
}
