using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class ColumnCollation : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtMost(TargetVersion.Version25);
    }

    public override string Source => @"
create table t (a varchar(20) character set utf8 collate unicode_ci);
";

    public override string Target => @"
create table t (a varchar(20) character set utf8 collate utf8);
";
}
