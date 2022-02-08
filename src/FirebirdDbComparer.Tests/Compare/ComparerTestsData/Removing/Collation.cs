namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class Collation : ComparerTests.TestCaseStructure
{
    public override string Source => @"
				
";

    public override string Target => @"
create collation iso8859_1_unicode for iso8859_1;
";
}
