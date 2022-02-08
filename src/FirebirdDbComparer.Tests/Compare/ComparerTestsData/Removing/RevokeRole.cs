namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class RevokeRole : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create role test_role;				
";

    public override string Target => @"
create role test_role;
grant test_role to test_user;
";
}
