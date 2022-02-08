namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class ColumnPKToPK_PKInline_PKUsingAlterWithName : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (id int not null);
alter table t add constraint pk_t primary key (id);				
";

    public override string Target => @"
create table t (id int primary key);
";
}
