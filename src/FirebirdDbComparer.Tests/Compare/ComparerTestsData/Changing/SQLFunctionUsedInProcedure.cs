using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class SQLFunctionUsedInProcedure : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
set term ^;

create function test(i bigint) returns bigint
as
begin
  return i + 2;
end^

create procedure call_func
returns (i bigint)
as
begin
  i = test(2);
  suspend;
end^

set term ;^				
";

    public override string Target => @"
set term ^;

create function test(i int) returns int
as
begin
  return i + 1;
end^

create procedure call_func
returns (i int)
as
begin
  i = test(1);
  suspend;
end^

set term ;^
";
}
