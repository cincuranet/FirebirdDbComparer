using System.Reflection;
using NUnitLite;

namespace FirebirdDbComparer.Tests
{
    public static class Program
    {
        public static void Main()
        {
            new AutoRun(Assembly.GetEntryAssembly()).Execute(new string[] { @"--where=test=~/ComparerTests40\./" });
        }
    }
}
