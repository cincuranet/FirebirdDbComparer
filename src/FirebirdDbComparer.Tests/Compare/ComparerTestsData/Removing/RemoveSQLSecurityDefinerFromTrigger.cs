using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class RemoveSQLSecurityDefinerFromTrigger : ComparerTests.TestCaseStructure
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
as
begin
end^

set term ;^
";

    public override string Target => @"
create table t (i int);

set term ^;

create trigger tr
for t
before insert
sql security definer
as
begin
end^

set term ;^
";
}
