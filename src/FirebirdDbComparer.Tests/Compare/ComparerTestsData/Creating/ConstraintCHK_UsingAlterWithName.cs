namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class ConstraintCHK_UsingAlterWithName : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (i int);
alter table t add constraint chk_t check (i > 0);				
";

    public override string Target => @"

";
}
