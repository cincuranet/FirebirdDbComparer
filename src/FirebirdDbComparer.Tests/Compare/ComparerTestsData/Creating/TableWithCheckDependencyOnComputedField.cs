namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class TableWithCheckDependencyOnComputedField : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (
    i int,
    j computed by (i * 1));
alter table t add constraint chk_j check (j > 0);				
";

    public override string Target => @"

";
}
