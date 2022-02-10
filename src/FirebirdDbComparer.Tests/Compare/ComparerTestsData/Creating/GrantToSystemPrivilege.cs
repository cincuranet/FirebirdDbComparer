using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class GrantToSystemPrivilege : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version40);
    }

    public override string Source => @"
create table test(foo int);
grant insert on test to system privilege modify_any_object_in_database;
";

    public override string Target => @"
create table test(foo int);
";
}
