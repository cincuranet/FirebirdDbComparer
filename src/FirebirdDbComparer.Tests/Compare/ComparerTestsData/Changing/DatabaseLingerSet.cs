using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class DatabaseLingerSet : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override void AssertScript(ScriptResult compareResult)
    {
        Assert.That(compareResult.AllStatements.Any(x => x.Contains(" LINGER ")), Is.True);
        Assert.That(compareResult.AllStatements.Any(x => x.Contains(" 10")), Is.True);
    }

    public override string Source => @"
alter database set linger to 10;
";

    public override string Target => @"

";
}
