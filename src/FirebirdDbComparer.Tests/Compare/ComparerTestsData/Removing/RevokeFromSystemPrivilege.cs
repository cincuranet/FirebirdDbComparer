using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class RevokeFromSystemPrivilege : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version40);
    }

    public override string Source => @"
create table test(foo int);
";

    public override string Target => @"
create table test(foo int);
grant delete on test to system privilege read_raw_pages;
";
}
