using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class ProcedureUsedInSQLFunction : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
set term ^;

create procedure test_procedure
returns (i bigint)
as
begin
  i = 2;
  suspend;
end^

create function call_proc() returns bigint
as
declare i bigint;
begin
  select sum(i) from test_procedure into i;
  return i;
end^

set term ;^				
";

    public override string Target => @"
set term ^;

create procedure test_procedure
returns (i int)
as
begin
  i = 1;
  suspend;
end^

create function call_proc() returns int
as
declare i int;
begin
  select sum(i) from test_procedure into i;
  return i;
end^

set term ;^
";
}
