using System.ComponentModel;

namespace FirebirdDbComparer.DatabaseObjects
{
    public enum Privilege
    {
        SELECT,
        UPDATE,
        DELETE,
        INSERT,
        EXECUTE,

        [Description("REFERENCES")]
        REFERENCE,
        MEMBER,
        USAGE,
        CREATE,
        ALTER,
        DROP
    }
}
