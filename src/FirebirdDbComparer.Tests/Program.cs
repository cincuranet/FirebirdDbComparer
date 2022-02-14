using System.Reflection;
using NUnitLite;

namespace FirebirdDbComparer.Tests;

public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 1)
        {
            return new AutoRun(Assembly.GetEntryAssembly()).Execute(new string[] { $@"--where=test=~/ComparerTests{args[0]}/" });
        }
        else
        {
            return new AutoRun(Assembly.GetEntryAssembly()).Execute(new string[] { @"--where=test=~/ComparerTests40\..*.*/" });
            //return new AutoRun(Assembly.GetEntryAssembly()).Execute(new string[] { @"--where=test=~/ComparerTests30\..*.*/" });
            //return new AutoRun(Assembly.GetEntryAssembly()).Execute(new string[] { @"--where=test=~/ComparerTests25\..*.*/" });
        }
    }
}
