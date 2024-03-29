namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class Index_DESC_MultipleFields_Unique : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (i int, j date);

create unique desc index idx
on t(i, j);				
";

    public override string Target => @"

";
}
