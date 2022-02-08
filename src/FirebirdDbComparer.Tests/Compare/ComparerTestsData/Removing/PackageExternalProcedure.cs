using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class PackageExternalProcedure : ComparerTests.TestCaseStructure
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
  procedure foo(in1 integer) returns (out1 integer);
end^

create package body some_pkg
as
begin
  procedure foo(in1 integer) returns (out1 integer) as begin out1 = 1; end
end^

set term ;^
";

    public override string Target => @"
set term ^;

create package some_pkg
as
begin
  procedure foo(in1 integer) returns (out1 integer);
  procedure new_ee_procedure(in1 integer) returns (out1 integer);
end^

create package body some_pkg
as
begin
  procedure foo(in1 integer) returns (out1 integer) as begin out1 = 1; end

  procedure new_ee_procedure(in1 integer) returns (out1 integer) external name 'FooBar!Foo.NewEEProcedure' engine FbNetExternalEngine;
end^

set term ;^				
";
}
