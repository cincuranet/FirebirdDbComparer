namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class Table : ComparerTests.TestCaseStructure
{
    public override string Source => @"
				
";

    public override string Target => @"
create table mytable (a int primary key);
";
}
