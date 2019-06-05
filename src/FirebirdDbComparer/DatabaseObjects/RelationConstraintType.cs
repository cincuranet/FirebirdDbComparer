using System.ComponentModel;

namespace FirebirdDbComparer.DatabaseObjects
{
    public enum RelationConstraintType
    {
        [Description("CHECK")]
        Check,

        [Description("FOREIGN KEY")]
        ForeignKey,

        [Description("NOT NULL")]
        NotNull,

        [Description("PRIMARY KEY")]
        PrimaryKey,

        [Description("UNIQUE")]
        Unique,
    }
}
