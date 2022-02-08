namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class Trigger_Connect_Active : ComparerTests.TestCaseStructure
{
    public override string Source => @"
set term ^;

create trigger trig
active
on connect
as
begin
end^

set term ;^				
";

    public override string Target => @"

";
}
