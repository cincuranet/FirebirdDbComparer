using System;

using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.Compare;

public sealed class ComparerSettings : IComparerSettings
{
    public ComparerSettings(TargetVersion targetVersion)
    {
        TargetVersion = targetVersion;
    }

    public TargetVersion TargetVersion { get; }
    public bool IgnorePermissions { get; set; }
}
