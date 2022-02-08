namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class Trigger_AI_Inactive : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (i int);

set term ^;

create trigger trig for t
inactive
after insert
as
begin
end^

set term ;^				
";

    public override string Target => @"

";
}
