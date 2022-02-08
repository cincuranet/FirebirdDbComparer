using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class PackageFunctionOnlyBody : ComparerTests.TestCaseStructure
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
  function test(i int) returns int;
end^

create package body some_pkg
as
begin
  function test(i int) returns int
  as
  begin
  end

  function change(i bigint) returns bigint
  as
  begin
    return 2;
  end
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

  function change(i int) returns int
  as
  begin
    return 2;
  end
end^

set term ;^
";
}
