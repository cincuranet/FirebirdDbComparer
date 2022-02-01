using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class PackageProcedureUsedInProcedure : ComparerTests.TestCaseStructure
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
  procedure test(i bigint);
end^

create package body some_pkg
as
begin
  procedure test(i bigint)
  as
  begin
  end
end^

create procedure foo
as
begin
  execute procedure some_pkg.test(1);
end^

set term ;^				
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

create procedure foo
as
begin
  execute procedure some_pkg.test(1);
end^

set term ;^
";
}
