using System;

using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Interfaces
{
    public interface IComparerSettings
    {
        TargetVersion TargetVersion { get; }
        bool IgnorePermissions { get; }
    }
}
