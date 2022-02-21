using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class PackageExternalProcedureSignature : ComparerTests.TestCaseStructure
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
  procedure new_ee_procedure(in1 integer) returns (out1 bigint);
end^

create package body some_pkg
as
begin
  procedure new_ee_procedure(in1 integer) returns (out1 bigint) external name 'FirebirdDbComparer.Tests.FooBar!Foo.NewEEProcedure' engine FbNetExternalEngine;
end^

set term ;^				
";

    public override string Target => @"
set term ^;

create package some_pkg
as
begin
  procedure new_ee_procedure(in1 integer) returns (out1 integer);
end^

create package body some_pkg
as
begin
  procedure new_ee_procedure(in1 integer) returns (out1 integer) external name 'FirebirdDbComparer.Tests.FooBar!Foo.NewEEProcedure' engine FbNetExternalEngine;
end^

set term ;^				
";
}
