using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class GrantRoleToRole : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version40);
    }

    public override string Source => @"
create role role1;
create role role2;
grant role1 to role2;				
";

    public override string Target => @"
create role role1;
create role role2;
";
}
