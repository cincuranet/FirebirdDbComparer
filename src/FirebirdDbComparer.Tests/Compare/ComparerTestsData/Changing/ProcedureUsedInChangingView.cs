namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class ProcedureUsedInChangingView : ComparerTests.TestCaseStructure
{
    public override string Source => @"
set term ^;

create procedure p (in_i int)
returns (out_i int)
as
begin
    out_i = in_i * 2;
    suspend;
end^

set term ;^

create view v
as
select (select out_i * 2 from p(0)) as i from rdb$database;				
";

    public override string Target => @"
set term ^ ;

create procedure p (in_i int)
returns (out_i int)
as
begin
    out_i = in_i;
    suspend;
end^

set term ;^

create view v
as
select (select out_i from p(0)) as i from rdb$database;
";
}
