namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class TriggerFromBeforeInsertToAfterUpdate : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (i int);

set term ^;

create trigger trig for t
after update
as
begin
end^

set term ;^				
";

    public override string Target => @"
create table t (i int);

set term ^;

create trigger trig for t
before insert
as
begin
end^

set term ;^
";
}
