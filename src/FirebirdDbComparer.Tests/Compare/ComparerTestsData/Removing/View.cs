namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class View : ComparerTests.TestCaseStructure
{
    public override string Source => @"
				
";

    public override string Target => @"
create view myview
as
select * from rdb$database;
";
}
