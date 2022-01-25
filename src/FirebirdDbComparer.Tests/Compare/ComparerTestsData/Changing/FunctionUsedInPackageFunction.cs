using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class FunctionUsedInPackageFunction : ComparerTests.TestCaseStructure
    {
        public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
        {
            return targetVersion.AtLeast(TargetVersion.Version30);
        }

        public override string Source => @"
set term ^;

create function foo(i bigint, j bigint) returns int
as
begin
  return 1;
end^

create package some_pkg
as
begin
  function test returns int;
end^

create package body some_pkg
as
begin
  function test returns int
  as
  begin
    return foo(1, 2);
  end
end^

set term ;^				
";

        public override string Target => @"
set term ^;

create function foo(i int) returns int
as
begin
  return 1;
end^

create package some_pkg
as
begin
  function test returns int;
end^

create package body some_pkg
as
begin
  function test returns int
  as
  begin
    return foo(1);
  end
end^

set term ;^
";
    }
}
