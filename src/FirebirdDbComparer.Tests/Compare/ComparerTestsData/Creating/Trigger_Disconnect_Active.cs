namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class Trigger_Disconnect_Active : ComparerTests.TestCaseStructure
{
    public override string Source => @"
set term ^;

create trigger trig
active
on disconnect
as
begin
end^

set term ;^				
";

    public override string Target => @"

";
}
