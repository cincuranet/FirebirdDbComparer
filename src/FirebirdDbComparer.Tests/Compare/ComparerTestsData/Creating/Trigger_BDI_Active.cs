namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class Trigger_BDI_Active : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (i int);

set term ^;

create trigger trig for t
active
before delete or insert
as
begin
end^

set term ;^				
";

    public override string Target => @"

";
}
