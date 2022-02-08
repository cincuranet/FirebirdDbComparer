namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class SimpleView : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create or alter view test_view
as
select * from rdb$database
;				
";

    public override string Target => @"

";
}
