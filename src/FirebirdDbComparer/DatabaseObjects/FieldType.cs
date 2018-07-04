namespace FirebirdDbComparer.DatabaseObjects
{
    public enum FieldType
    {
        UNKNOWN = 0,
        SHORT = 7,
        LONG = 8,
        QUAD = 9,
        FLOAT = 10,
        DATE = 12,
        TIME = 13,
        TEXT = 14,
        INT64 = 16,
        BOOLEAN = 23,
        DOUBLE = 27,
        TIMESTAMP = 35,
        VARYING = 37,
        CSTRING = 40,
        BLOB_ID = 45,
        BLOB = 261
    }
}
