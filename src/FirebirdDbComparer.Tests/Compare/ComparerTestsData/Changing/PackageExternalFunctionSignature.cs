using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class PackageExternalFunctionSignature : ComparerTests.TestCaseStructure
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
  function new_ee_function(in1 integer) returns bigint;
end^

create package body some_pkg
as
begin
  function new_ee_function(in1 integer) returns bigint external name 'FirebirdDbComparer.Tests.FooBar!Foo.NewEEFunction' engine FbNetExternalEngine;
end^

set term ;^				
";

    public override string Target => @"
set term ^;

create package some_pkg
as
begin
  function new_ee_function(in1 integer) returns integer;
end^

create package body some_pkg
as
begin
  function new_ee_function(in1 integer) returns integer external name 'FirebirdDbComparer.Tests.FooBar!Foo.NewEEFunction' engine FbNetExternalEngine;
end^

set term ;^				
";
}
