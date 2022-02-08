namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class UNQConstraintToUNQIndex : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (i int);
create unique index unq_i on t(i);				
";

    public override string Target => @"
create table t (i int);
alter table t add constraint unq_i unique(i) using index unq_i;
";
}
