using System;

namespace FirebirdDbComparer.Interfaces
{
    public interface IComparerFactory : IDisposable
    {
        IComparer Create(IComparerSettings comparerSettings, IMetadata sourceMetadata, IMetadata targetMetadata);
    }
}
