namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class TableColumnPosition : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (a int, b smallint, c bigint);				
";

    public override string Target => @"
create table t (a int, c bigint, b smallint);
";
}
