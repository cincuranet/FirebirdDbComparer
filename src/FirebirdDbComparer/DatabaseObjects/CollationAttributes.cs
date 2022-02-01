using System;

namespace FirebirdDbComparer.DatabaseObjects;

[Flags]
public enum CollationAttributes
{
    PadSpace = 1,
    CaseInsensitive = 2,
    AccentInsensitive = 4,
}
