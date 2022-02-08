namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class TableAndViewAndProcedure_ViewWithTriggerUsingThatProcedure : ComparerTests.TestCaseStructure
{
    public override string Source => @"
				
";

    public override string Target => @"
create table t (a varchar(20));

create view v
as
select a from t;

set term ^;

create procedure p (in_a varchar(20))
returns (out_a varchar(20))
as
begin
    out_a = in_a;
end^

create trigger trig for v
before update
as
declare var_a varchar(20);
begin
    execute procedure p(new.a) returning_values (var_a);
    new.a = var_a;
end^

set term ;^
";
}
