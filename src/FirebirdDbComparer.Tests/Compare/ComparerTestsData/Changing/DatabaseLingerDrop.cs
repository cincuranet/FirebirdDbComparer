using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class DatabaseLingerDrop : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override void AssertScript(ScriptResult compareResult)
    {
        Assert.That(compareResult.AllStatements.Any(x => x.Contains(" LINGER ")), Is.True);
        Assert.That(compareResult.AllStatements.Any(x => x.Contains(" 0")), Is.True);
    }

    public override string Source => @"

";

    public override string Target => @"
alter database set linger to 10;
";
}
