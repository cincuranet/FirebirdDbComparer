namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class DomainCharVarcharLength_DifferentCollate : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create domain d1 as char(2) collate unicode_ci_ai;
create domain d2 as varchar(2) collate unicode_ci_ai;

create table t (a d1, b d2);				
";

    public override string Target => @"
create domain d1 as char(1) collate unicode_ci_ai;
create domain d2 as varchar(1) collate unicode_ci_ai;

create table t (a d1, b d2);
";
}
