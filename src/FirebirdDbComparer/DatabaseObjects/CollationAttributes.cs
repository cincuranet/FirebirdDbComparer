using System;

namespace FirebirdDbComparer.DatabaseObjects
{
    [Flags]
    public enum CollationAttributes
    {
        TEXTTYPE_ATTR_PAD_SPACE = 1,
        TEXTTYPE_ATTR_CASE_INSENSITIVE = 2,
        TEXTTYPE_ATTR_ACCENT_INSENSITIVE = 4
    }
}
