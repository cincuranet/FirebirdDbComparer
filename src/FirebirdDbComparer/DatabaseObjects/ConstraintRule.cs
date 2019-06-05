using System.ComponentModel;

namespace FirebirdDbComparer.DatabaseObjects
{
    public enum ConstraintRule
    {
        Restrict,

        [Description("CASCADE")]
        Cascade,

        [Description("SET DEFAULT")]
        SetDefault,

        [Description("SET NULL")]
        SetNull,

        [Description("NO ACTION")]
        NoAction,
    }
}
