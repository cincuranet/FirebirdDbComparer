namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class Trigger_TxStart_Inactive : ComparerTests.TestCaseStructure
{
    public override string Source => @"
set term ^;

create trigger trig
inactive
on transaction start
as
begin
end^

set term ;^				
";

    public override string Target => @"

";
}
