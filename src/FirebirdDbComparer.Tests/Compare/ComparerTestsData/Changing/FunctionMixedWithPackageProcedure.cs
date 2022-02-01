using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class FunctionMixedWithPackageProcedure : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
set term ^ ;

create function p (in_i int)
returns int
as
begin
    return in_i * 2;
end^

set term ;^				
";

    public override string Target => @"
set term ^ ;

create function p (in_i int)
returns int
as
begin
    return in_i * 2;
end^

create package pkg
as
begin
    function p (in_i int) returns int;
end^
create package body pkg
as
begin
    function p (in_i int)
    returns int
    as
    begin
        return in_i * 2;
    end
end^

set term ;^
";
}
