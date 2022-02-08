using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class ProcedureMixedWithPackageProcedure : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
set term ^ ;

create procedure p (in_i int)
returns (out_i int)
as
begin
    out_i = in_i * 2;
end^

set term ;^				
";

    public override string Target => @"
set term ^ ;

create procedure p (in_i int)
returns (out_i int)
as
begin
    out_i = in_i * 2;
end^

create package pkg
as
begin
    procedure p (in_i int) returns (out_i int);
end^
create package body pkg
as
begin
    procedure p (in_i int)
    returns (out_i int)
    as
    begin
        out_i = in_i * 2;
    end
end^

set term ;^
";
}
