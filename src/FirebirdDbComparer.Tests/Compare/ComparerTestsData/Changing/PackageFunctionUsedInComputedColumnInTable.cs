using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

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
  function test(i int) returns bigint;
end^

create package body some_pkg
as
begin
  function test(i int) returns bigint
  as
  begin
    return 1;
  end
end^

set term ;^

create table test (i int, c computed by (some_pkg.test(i)));;				
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

set term ;^

create table test (i int, c computed by (some_pkg.test(i)));
";
}
