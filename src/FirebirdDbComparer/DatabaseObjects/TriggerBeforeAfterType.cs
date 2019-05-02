using System.ComponentModel;

namespace FirebirdDbComparer.DatabaseObjects
{
    public enum TriggerBeforeAfterType
    {
        [Description("BEFORE")]
        Before,

        [Description("AFTER")]
        After,
    }
}
