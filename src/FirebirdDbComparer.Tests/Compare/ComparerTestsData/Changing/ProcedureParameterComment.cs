namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class ProcedureParameterComment : ComparerTests.TestCaseStructure
{
    public override string Source => @"
set term ^;

create or alter procedure new_procedure (a int)
returns (b int)
as
begin
end^

set term ;^

comment on parameter new_procedure.a is 'a_new';
comment on parameter new_procedure.b is 'b_new';				
";

    public override string Target => @"
set term ^;

create or alter procedure new_procedure (a int)
returns (b int)
as
begin
end^

set term ;^

comment on parameter new_procedure.a is 'a';
comment on parameter new_procedure.b is 'b';
";
}
