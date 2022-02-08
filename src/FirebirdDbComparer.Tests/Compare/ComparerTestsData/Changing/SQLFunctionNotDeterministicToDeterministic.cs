using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class SQLFunctionNotDeterministicToDeterministic : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
set term ^;

create function test(i int) returns int deterministic
as
begin
  return i;
end^

set term ;^				
";

    public override string Target => @"
set term ^;

create function test(i int) returns int
as
begin
  return i;
end^

set term ;^
";
}
