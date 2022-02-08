namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class DomainWithDependency : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (b char(20));				
";

    public override string Target => @"
create domain d_string as varchar(20) character set utf8;
create table t (a d_string, b char(20));
";
}
