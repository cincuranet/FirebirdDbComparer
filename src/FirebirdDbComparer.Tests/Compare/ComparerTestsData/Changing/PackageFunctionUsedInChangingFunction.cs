using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class PackageFunctionUsedInChangingFunction : ComparerTests.TestCaseStructure
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
  function test(i bigint) returns bigint;
end^

create package body some_pkg
as
begin
  function test(i bigint) returns bigint
  as
  begin
    return 1;
  end
end^

create function foo(i bigint) returns bigint
as
begin
  return some_pkg.test(i);
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

create function foo(i int) returns int
as
begin
  return some_pkg.test(i);
end^

set term ;^
";
}
