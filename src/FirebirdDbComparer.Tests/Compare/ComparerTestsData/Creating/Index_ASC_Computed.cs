namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class Index_ASC_Computed : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (i int);

create asc index idx
on t computed by (i*2);				
";

    public override string Target => @"

";
}
