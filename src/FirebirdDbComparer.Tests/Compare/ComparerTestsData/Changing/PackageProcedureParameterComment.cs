using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class PackageProcedureParameterComment : ComparerTests.TestCaseStructure
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
  procedure test(i int) returns (j int);
end^

create package body some_pkg
as
begin
  procedure test(i int) returns (j int)
  as
  begin
  end
end^

set term ;^

comment on parameter some_pkg.test.i is 'pkg_new';
comment on parameter some_pkg.test.j is 'pkg2_new';				
";

    public override string Target => @"
set term ^;

create package some_pkg
as
begin
  procedure test(i int) returns (j int);
end^

create package body some_pkg
as
begin
  procedure test(i int) returns (j int)
  as
  begin
  end
end^

set term ;^

comment on parameter some_pkg.test.i is 'pkg';
comment on parameter some_pkg.test.j is 'pkg2';
";
}
