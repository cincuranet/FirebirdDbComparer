using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Interfaces
{
    public interface IComparer
    {
        IComparerContext ComparerContext { get; }
        IMetadata SourceMetadata { get; }
        IMetadata TargetMetadata { get; }
        CompareResult Compare();
    }
}
