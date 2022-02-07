using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class PackageExternalFunction : ComparerTests.TestCaseStructure
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
  function foo(in1 integer) returns integer;
end^

create package body some_pkg
as
begin
  function foo(in1 integer) returns integer as begin return 1; end
end^

set term ;^
";

    public override string Target => @"
set term ^;

create package some_pkg
as
begin
  function foo(in1 integer) returns integer;
  function new_ee_function(in1 integer) returns integer;
end^

create package body some_pkg
as
begin
  function foo(in1 integer) returns integer as begin return 1; end

  function new_ee_function(in1 integer) returns integer external name 'FooBar!Foo.NewEEFunction' engine FbNetExternalEngine;
end^

set term ;^				
";
}
