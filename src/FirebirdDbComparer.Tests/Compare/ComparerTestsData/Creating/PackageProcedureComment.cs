using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class PackageProcedureComment : ComparerTests.TestCaseStructure
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
  procedure test(i int);
end^

create package body some_pkg
as
begin
  procedure test(i int)
  as
  begin
  end
end^

set term ;^

comment on procedure some_pkg.test is 'test';				
";

    public override string Target => @"
set term ^;

create package some_pkg
as
begin
  procedure test(i int);
end^

create package body some_pkg
as
begin
  procedure test(i int)
  as
  begin
  end
end^

set term ;^
";
}
