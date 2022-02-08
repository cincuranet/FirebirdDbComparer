namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class Index_ASC_OneField_Inactive : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (i int);

create asc index idx
on t(i);
alter index idx inactive;				
";

    public override string Target => @"

";
}
