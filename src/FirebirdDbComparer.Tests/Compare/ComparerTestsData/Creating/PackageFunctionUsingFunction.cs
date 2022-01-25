using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class PackageFunctionUsingFunction : ComparerTests.TestCaseStructure
    {
        public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
        {
            return targetVersion.AtLeast(TargetVersion.Version30);
        }

        public override string Source => @"
set term ^;

create function foo(i int) returns int
as
begin
  return 1;
end^

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
    return foo(i);
  end
end^

set term ;^				
";

        public override string Target => @"

";
    }
}
