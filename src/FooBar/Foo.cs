using System;
using System.Collections.Generic;

public static class Foo
{
    public static IEnumerator<ValueTuple<long?>> NewEEProcedure(long? in1)
    {
        yield break;
    }

    public static IEnumerator<ValueTuple<long?>> NewEEProcedure2(long? in1)
    {
        yield break;
    }

    public static long? NewEEFunction(long? in1)
    {
        return default;
    }

    public static long? NewEEFunction2(long? in1)
    {
        return default;
    }

    public static string DllVersion(long? dummy)
    {
        return default;
    }
}
