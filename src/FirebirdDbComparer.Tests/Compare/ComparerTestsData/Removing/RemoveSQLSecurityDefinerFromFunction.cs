using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class RemoveSQLSecurityDefinerFromFunction : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version40);
    }

    public override string Source => @"
set term ^;

create function f
returns int
as
begin
    return 6;
end^

set term ;^
";

    public override string Target => @"
set term ^;

create function f
returns int
sql security definer
as
begin
    return 6;
end^

set term ;^
";
}
