using System.ComponentModel;

namespace FirebirdDbComparer.DatabaseObjects;

public enum IdentityTypeType
{
    Always = 0,

    [Description("BY DEFAULT")]
    ByDefault = 1,
}
