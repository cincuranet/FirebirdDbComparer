namespace FirebirdDbComparer.DatabaseObjects;

public enum FieldType
{
    Unknown = 0,
    Short = 7,
    Long = 8,
    Quad = 9,
    Float = 10,
    Date = 12,
    Time = 13,
    Text = 14,
    Int64 = 16,
    Boolean = 23,
    DecFloat16 = 24,
    DecFloat34 = 25,
    Int128 = 26,
    Double = 27,
    TimeWithTimeZone = 28,
    TimestampWithTimeZone = 29,
    Timestamp = 35,
    Varying = 37,
    CString = 40,
    BlobId = 45,
    Blob = 261,
}
