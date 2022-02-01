using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class SQLFunctionUsingChangingFunction : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
set term ^;

create function testA(i int, j int) returns int
as
begin
  return i + 1;
end^

create function testB(i int) returns int
as
begin
  return i + testA(i, i);
end^

set term ;^				
";

    public override string Target => @"
set term ^;

create function testA(i int) returns int
as
begin
  return i + 1;
end^

create function testB(i int) returns int
as
begin
  return i + testA(i);
end^

set term ;^
";
}
