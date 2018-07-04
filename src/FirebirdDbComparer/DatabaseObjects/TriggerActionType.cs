using System.ComponentModel;

namespace FirebirdDbComparer.DatabaseObjects
{
    public enum TriggerActionType
    {
        [Description("INSERT")]
        BEFORE_INSERT,

        [Description("INSERT")]
        AFTER_INSERT,

        [Description("UPDATE")]
        BEFORE_UPDATE,

        [Description("UPDATE")]
        AFTER_UPDATE,

        [Description("DELETE")]
        BEFORE_DELETE,

        [Description("DELETE")]
        AFTER_DELETE,

        [Description("CONNECT")]
        CONNECT,

        [Description("DISCONNECT")]
        DISCONNECT,

        [Description("TRANSACTION START")]
        TRANS_START,

        [Description("TRANSACTION COMMIT")]
        TRANS_COMMIT,

        [Description("TRANSACTION ROLLBACK")]
        TRANS_ROLLBACK
    }
}
