using System;

namespace FirebirdDbComparer.Exceptions;

/// <summary>
/// Whenever you remove this exception, think *hard* about dependencies!
/// </summary>
public class CrossTypesOfSameObjectTypesException : NotSupportedException
{
    public CrossTypesOfSameObjectTypesException()
        : base("Objects share same name but are of different type. Not yet supported.")
    { }
}
