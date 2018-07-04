using System;

namespace FirebirdDbComparer.DatabaseObjects
{
    [Flags]
    public enum RoleFlagType
    {
        ROLE_FLAG_MAY_TRUST = 2,
        ROLE_FLAG_DBO = 4
    }
}
