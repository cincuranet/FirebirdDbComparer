using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class PackageFunctionUsedInComputedColumnInTable : ComparerTests.TestCaseStructure
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
  function b(i int) returns int;
end^

create package body some_pkg
as
begin
  function b(i int) returns int
  as
  begin
    return 1;
  end
end^

set term ;^

create table test (i int, c computed by (some_pkg.b(i)));				
";

    public override string Target => @"
set term ^;

create package some_pkg
as
begin
  function a(i int) returns int;
end^

create package body some_pkg
as
begin
  function a(i int) returns int
  as
  begin
    return 1;
  end
end^

set term ;^

create table test (i int, c computed by (some_pkg.a(i)));
";
}
