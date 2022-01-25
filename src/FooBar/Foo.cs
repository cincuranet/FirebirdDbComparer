using System;
using System.Collections.Generic;

public static class foo
{
    public static IEnumerator<ValueTuple<long?>> NewEEProcedure(long? in1)
    {
        yield break;
    }

    public static long? NewEEFunction(long? in1)
    {
        return default;
    }
}
