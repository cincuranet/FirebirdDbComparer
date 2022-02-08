namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class ColumnComputed : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (a char(1), b computed by (trim(a)));				
";

    public override string Target => @"
create table t (a char(1), b computed by (trim(a || a)));
";
}
