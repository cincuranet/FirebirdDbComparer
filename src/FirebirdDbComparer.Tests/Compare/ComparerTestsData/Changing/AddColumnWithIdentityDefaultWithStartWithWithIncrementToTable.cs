using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class AddColumnWithIdentityDefaultWithStartWithWithIncrementToTable : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version40);
    }

    public override string Source => @"
create table t (a int, b bigint generated by default as identity (start with 99 increment by 6));				
";

    public override string Target => @"
create table t (a int);
";
}
