using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class SQLFunctionUsedInTrigger : ComparerTests.TestCaseStructure
    {
        public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
        {
            return targetVersion.AtLeast(TargetVersion.Version30);
        }

        public override string Source => @"
create table t(i int);

set term ^;

create function test(i int, j int) returns boolean
as
begin
  return true;
end^

create trigger trig for t
before insert
as
begin
  if (test(new.i, new.i)) then
  begin
  end
end^

set term ;^				
";

        public override string Target => @"
create table t(i int);

set term ^;

create function test(i int) returns boolean
as
begin
  return true;
end^

create trigger trig for t
before insert
as
begin
  if (test(new.i)) then
  begin
  end
end^

set term ;^
";
    }
}
