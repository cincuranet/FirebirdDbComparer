using System.ComponentModel;

namespace FirebirdDbComparer.DatabaseObjects
{
    public enum RelationConstraintType
    {
        [Description("CHECK")]
        CHECK,

        [Description("FOREIGN KEY")]
        FOREIGN_KEY,

        [Description("NOT NULL")]
        NOT_NULL,

        [Description("PRIMARY KEY")]
        PRIMARY_KEY,

        [Description("UNIQUE")]
        UNIQUE
    }
}
