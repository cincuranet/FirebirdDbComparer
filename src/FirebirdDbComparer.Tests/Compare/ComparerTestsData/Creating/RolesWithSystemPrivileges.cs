namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class RolesWithSystemPrivileges : ComparerTests.TestCaseStructure
{
    public override string Source => @"
CREATE ROLE r_test SET SYSTEM PRIVILEGES TO CREATE_DATABASE, DROP_DATABASE;				
";

    public override string Target => @"

";
}
