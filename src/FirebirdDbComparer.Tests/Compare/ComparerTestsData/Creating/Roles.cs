namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class Roles : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create role r_normal;				
";

    public override string Target => @"

";
}
