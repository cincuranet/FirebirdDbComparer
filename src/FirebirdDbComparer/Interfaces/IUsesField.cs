using FirebirdDbComparer.DatabaseObjects.Primitives;

namespace FirebirdDbComparer.Interfaces
{
    public interface IUsesField
    {
        Field Field { get; }
    }
}
