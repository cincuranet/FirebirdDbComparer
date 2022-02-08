namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class AddComputedColumnToTable : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (a int, b computed by (a+a));				
";

    public override string Target => @"
create table t (a int);
";
}
