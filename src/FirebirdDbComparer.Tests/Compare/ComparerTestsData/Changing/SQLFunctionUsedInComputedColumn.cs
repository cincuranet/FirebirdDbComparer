using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class SQLFunctionUsedInComputedColumn : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
set term ^;

create function test(i int, j int) returns int
as
begin
  return i + 2;
end^

set term ;^

create table t(i int, j computed by (test(i, i)));				
";

    public override string Target => @"
set term ^;

create function test(i int) returns int
as
begin
  return i + 1;
end^

set term ;^

create table t(i int, j computed by (test(i)));
";
}
