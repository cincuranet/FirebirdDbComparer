namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class UpdatableViewWithProcedureDepenedency : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (i int);

create view v
as
select i from t;

set term ^;

create trigger trg_v for v
active
before insert or update or delete
as
begin
end^

create procedure p
as
begin
    insert into v(i) values (0);
end^

set term ;^				
";

    public override string Target => @"

";
}
