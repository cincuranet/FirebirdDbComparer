using System;

namespace FirebirdDbComparer.DatabaseObjects;

public enum FunctionArgumentMechanism
{
    ByValue = 0,
    ByReference = 1,
    ByVMSDescriptor = 2,
    ByISCDescriptor = 3,
    ByScalarArrayDescriptor = 4,
    ByReferenceWithNull = 5,
}
