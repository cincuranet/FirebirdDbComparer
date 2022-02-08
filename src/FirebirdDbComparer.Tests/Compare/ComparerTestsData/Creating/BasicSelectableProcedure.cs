namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class BasicSelectableProcedure : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create domain d_test as int;

set term ^;

create or alter procedure new_procedure (
    in1 varchar(10) character set utf8 collate ucs_basic,
    in2 d_test,
    in3 type of d_test)
returns (
    out1 integer,
    out2 d_test,
    out3 type of d_test)
as
declare var integer;
begin
  suspend;
end^

set term ;^				
";

    public override string Target => @"

";
}
