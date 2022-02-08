using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class RoleRemoveSystemPrivilege : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version40);
    }

    public override string Source => @"
CREATE ROLE r_test SET SYSTEM PRIVILEGES TO CREATE_DATABASE;				
";

    public override string Target => @"
CREATE ROLE r_test SET SYSTEM PRIVILEGES TO CREATE_DATABASE, DROP_DATABASE;	
";
}
