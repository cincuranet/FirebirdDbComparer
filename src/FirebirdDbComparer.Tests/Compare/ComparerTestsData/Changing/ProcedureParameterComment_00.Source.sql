set term ^;

create or alter procedure new_procedure (a int)
returns (b int)
as
begin
end^

set term ;^

comment on parameter new_procedure.a is 'a_new';
comment on parameter new_procedure.b is 'b_new';