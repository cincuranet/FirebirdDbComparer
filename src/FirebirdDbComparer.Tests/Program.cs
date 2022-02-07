using System.Reflection;
using NUnitLite;

namespace FirebirdDbComparer.Tests;

public static class Program
{
    public static int Main()
    {
        return new AutoRun(Assembly.GetEntryAssembly()).Execute(new string[] { @"--where=test=~/ComparerTests40\..*.*/" });
    }
}
