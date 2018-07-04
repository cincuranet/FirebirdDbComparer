using System.ComponentModel;

namespace FirebirdDbComparer.DatabaseObjects
{
    public enum ConstraintRule
    {
        RESTRICT,

        [Description("CASCADE")]
        CASCADE,

        [Description("SET DEFAULT")]
        SET_DEFAULT,

        [Description("SET NULL")]
        SET_NULL,

        [Description("NO ACTION")]
        NO_ACTION
    }
}
