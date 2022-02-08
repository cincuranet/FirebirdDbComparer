namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class CollateUsageHandling : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create domain d_string as varchar(20) character set utf8 collate unicode_ci;
create table t (
    a varchar(20) character set utf8,
    b varchar(20) character set utf8 collate unicode_ci,
    c d_string,
    d d_string collate unicode_ci,
    e d_string collate unicode);				
";

    public override string Target => @"

";
}
