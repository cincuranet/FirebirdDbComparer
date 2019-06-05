using System.ComponentModel;

namespace FirebirdDbComparer.DatabaseObjects
{
    public enum MetadataRelationType
    {
        Persistent = 0,
        View = 1,
        External = 2,
        Virtual = 3,

        [Description("PRESERVE")]
        GlobalTemporaryPreserve = 4,

        [Description("DELETE")]
        GlobalTemporaryDelete = 5,
    }
}
