using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class PackageFunctionUsedInFunction : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
set term ^;

create package some_pkg
as
begin
  function test(i bigint, j bigint) returns int;
end^

create package body some_pkg
as
begin
  function test(i bigint, j bigint) returns int
  as
  begin
  	return 1;
  end
end^

create function foo returns int
as
begin
  return some_pkg.test(1, 2);
end^

set term ;^				
";

    public override string Target => @"
set term ^;

create package some_pkg
as
begin
  function test(i int) returns int;
end^

create package body some_pkg
as
begin
  function test(i int) returns int
  as
  begin
    return 1;
  end
end^

create function foo returns int
as
begin
  return some_pkg.test(1);
end^

set term ;^
";
}
