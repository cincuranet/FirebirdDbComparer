namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class ColumnPKToUQ_BothUsingAlterWithName : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (id int not null);
alter table t add constraint uq_t unique (id);				
";

    public override string Target => @"
create table t (id int not null);
alter table t add constraint pk_t primary key (id);
";
}
