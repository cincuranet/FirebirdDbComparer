using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class DatabaseSQLSecurityDefiner : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version40);
    }

    public override void AssertScript(ScriptResult compareResult)
    {
        Assert.That(compareResult.AllStatements.Any(x => x.Contains(" SQL SECURITY ")), Is.True);
    }

    public override string Source => @"
alter database set default sql security definer;
";

    public override string Target => @"

";
}
