namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class ColumnWithFK_Inline : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t_master (a int primary key);
create table t_detail (a int primary key);				
";

    public override string Target => @"
create table t_master (a int primary key);
create table t_detail (a int primary key, drop_me int references t_master(a));
";
}
