namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class ProcedureUsedInTrigger : ComparerTests.TestCaseStructure
{
    public override string Source => @"
set term ^ ;

create procedure p (in_i int)
returns (out_i int)
as
begin
    out_i = in_i * 2;
end^

set term ;^

create table t (i int);

set term ^;

create trigger trig for t
before insert
as
begin
    execute procedure p(new.i) returning_values new.i;
end^

set term ;^				
";

    public override string Target => @"
set term ^ ;

create procedure p (in_i int)
returns (out_i int)
as
begin
    out_i = in_i;
end^

set term ;^

create table t (i int);

set term ^;

create trigger trig for t
before insert
as
begin
    execute procedure p(new.i) returning_values new.i;
end^

set term ;^
";
}
