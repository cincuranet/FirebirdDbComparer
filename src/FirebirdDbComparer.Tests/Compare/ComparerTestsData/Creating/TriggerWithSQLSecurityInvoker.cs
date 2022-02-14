using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class TriggerWithSQLSecurityInvoker : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version40);
    }

    public override string Source => @"
create table t (i int);

set term ^;

create trigger tr
for t
before insert
sql security invoker
as
begin
end^

set term ;^
";

    public override string Target => @"
create table t (i int);
";
}
