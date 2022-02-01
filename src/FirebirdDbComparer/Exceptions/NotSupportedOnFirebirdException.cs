using System;

namespace FirebirdDbComparer.Exceptions;

public class NotSupportedOnFirebirdException : NotSupportedException
{
    public NotSupportedOnFirebirdException(string message)
        : base(message)
    { }
}
