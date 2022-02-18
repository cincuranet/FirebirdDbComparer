using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class DatabaseSQLSecurityInvoker : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version40);
    }

    public override void AssertScript(ScriptResult compareResult)
    {
        Assert.That(compareResult.AllStatements, Is.Empty);
    }

    public override string Source => @"
alter database set default sql security invoker;
";

    public override string Target => @"

";
}
