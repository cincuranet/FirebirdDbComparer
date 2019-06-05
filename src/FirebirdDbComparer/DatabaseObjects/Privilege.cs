using System.ComponentModel;

namespace FirebirdDbComparer.DatabaseObjects
{
    public enum Privilege
    {
        Select,
        Update,
        Delete,
        Insert,
        Execute,

        [Description("REFERENCES")]
        Reference,
        Member,
        Usage,
        Create,
        Alter,
        Drop,
    }
}
