using System;

namespace FirebirdDbComparer.Exceptions
{
    class DeadCodePathException : Exception
    {
        public DeadCodePathException()
            : base("This code path is dead and should be handled on other place.")
        { }
    }
}
