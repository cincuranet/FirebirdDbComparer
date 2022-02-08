using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class RemoveDefaultFromRole : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version40);
    }

    public override string Source => @"
create role test_role;		
grant test_role to test_user;	
";

    public override string Target => @"
create role test_role;
grant default test_role to test_user;
";
}
