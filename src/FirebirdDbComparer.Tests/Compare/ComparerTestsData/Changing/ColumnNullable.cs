namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class ColumnNullable : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (a char(1) not null, b char(1));				
";

    public override string Target => @"
create table t (a char(1), b char(1) not null);
";
}
