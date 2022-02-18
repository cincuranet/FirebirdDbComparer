using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class DatabaseLingerSet0Nothing : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override void AssertScript(ScriptResult compareResult)
    {
        Assert.That(compareResult.AllStatements, Is.Empty);
    }

    public override string Source => @"
alter database set linger to 0;
";

    public override string Target => @"

";
}
