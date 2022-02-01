using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class PackageFunctionArgumentComment : ComparerTests.TestCaseStructure
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
    function test(i int) returns int;
end^

create package body some_pkg
as
begin
    function test(i int) returns int
    as
    begin
        return i;
    end
end^

set term ;^

comment on parameter some_pkg.test.i is 'pkg';				
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
        return i;
    end
end^

set term ;^
";
}
