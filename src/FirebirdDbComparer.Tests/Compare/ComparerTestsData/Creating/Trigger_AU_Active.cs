namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class Trigger_AU_Active : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (i int);

set term ^;

create trigger trig for t
active
after update
as
begin
end^

set term ;^				
";

    public override string Target => @"

";
}
