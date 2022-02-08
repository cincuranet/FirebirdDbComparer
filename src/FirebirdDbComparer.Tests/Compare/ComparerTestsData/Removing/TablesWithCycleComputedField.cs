namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class TablesWithCycleComputedField : ComparerTests.TestCaseStructure
{
    public override string Source => @"
				
";

    public override string Target => @"
create table t1 (a float);
create table t2 (a float);
alter table t1 add b computed by ((select first 1 a from t2));
alter table t2 add b computed by ((select first 1 a from t1));
";
}
