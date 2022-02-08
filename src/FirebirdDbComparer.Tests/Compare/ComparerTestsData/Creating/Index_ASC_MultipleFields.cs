namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class Index_ASC_MultipleFields : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (i int, j date);

create asc index idx
on t(i, j);				
";

    public override string Target => @"

";
}
