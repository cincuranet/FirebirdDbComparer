using System;

namespace FirebirdDbComparer.DatabaseObjects
{
    [Flags]
    public enum RoleFlagType
    {
        RoleFlagMayTrust = 2,
        RoleFlagDBO = 4,
    }
}
