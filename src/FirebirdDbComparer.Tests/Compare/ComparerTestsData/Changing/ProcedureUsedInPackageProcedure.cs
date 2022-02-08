using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class ProcedureUsedInPackageProcedure : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
set term ^;

create procedure foo(i bigint)
as
begin
end^

create package some_pkg
as
begin
  procedure test;
end^

create package body some_pkg
as
begin
  procedure test
  as
  begin
    execute procedure foo(1);
  end
end^

set term ;^				
";

    public override string Target => @"
set term ^;

create procedure foo(i int)
as
begin
end^

create package some_pkg
as
begin
  procedure test;
end^

create package body some_pkg
as
begin
  procedure test
  as
  begin
    execute procedure foo(1);
  end
end^

set term ;^
";
}
