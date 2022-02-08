namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class Trigger_BU_Inactive : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (i int);

set term ^;

create trigger trig for t
inactive
before update
as
begin
end^

set term ;^				
";

    public override string Target => @"

";
}
