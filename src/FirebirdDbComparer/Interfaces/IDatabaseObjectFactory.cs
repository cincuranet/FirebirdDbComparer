using System;

namespace FirebirdDbComparer.Interfaces
{
    public interface IDatabaseObjectFactory : IDisposable
    {
        IDatabaseObject[] ResolveAll(IMetadata metadata);
    }
}
