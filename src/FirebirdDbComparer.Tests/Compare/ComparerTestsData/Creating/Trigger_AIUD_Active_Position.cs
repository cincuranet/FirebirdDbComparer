namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class Trigger_AIUD_Active_Position : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (i int);

set term ^;

create trigger trig for t
active
after insert or update or delete
position 222
as
begin
end^

set term ;^				
";

    public override string Target => @"

";
}
