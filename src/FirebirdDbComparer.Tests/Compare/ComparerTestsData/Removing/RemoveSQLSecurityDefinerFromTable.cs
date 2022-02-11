using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class RemoveSQLSecurityDefinerFromTable : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version40);
    }

    public override string Source => @"
create table t (i int);
";

    public override string Target => @"
create table t (i int) sql security definer;
";
}
