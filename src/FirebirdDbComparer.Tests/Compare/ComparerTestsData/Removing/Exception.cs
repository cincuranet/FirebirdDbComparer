namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class Exception : ComparerTests.TestCaseStructure
{
    public override string Source => @"
				
";

    public override string Target => @"
create exception ex_dummy 'dummy';
";
}
