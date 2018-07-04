using System;

namespace FirebirdDbComparer.DatabaseObjects
{
    public enum FunctionArgumentMechanism
    {
        BY_VALUE = 0,
        BY_REFERENCE = 1,
        BY_VMS_DESCRIPTOR = 2,
        BY_ISC_DESCRIPTOR = 3,
        BY_SCALAR_ARRAY_DESCRIPTOR = 4,
        BY_REFERENCE_WITH_NULL = 5
    }
}
