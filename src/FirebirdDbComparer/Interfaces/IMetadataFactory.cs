using System;

namespace FirebirdDbComparer.Interfaces;

public interface IMetadataFactory : IDisposable
{
    IMetadata Create(string connectionString);
}
