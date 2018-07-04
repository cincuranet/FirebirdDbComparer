using System.ComponentModel;

namespace FirebirdDbComparer.DatabaseObjects
{
    public enum MetadataRelationType
    {
        PERSISTENT = 0,
        VIEW = 1,
        EXTERNAL = 2,
        VIRTUAL = 3,

        [Description("PRESERVE")]
        GLOBAL_TEMPORARY_PRESERVE = 4,

        [Description("DELETE")]
        GLOBAL_TEMPORARY_DELETE = 5
    }
}
