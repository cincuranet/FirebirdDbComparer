namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class TriggerBody : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (i int);

set term ^;

create trigger trig for t
before update
as
begin
    new.i = old.i;
end^

set term ;^				
";

    public override string Target => @"
create table t (i int);

set term ^;

create trigger trig for t
before update
as
begin
    new.i = old.i * 2;
end^

set term ;^
";
}
