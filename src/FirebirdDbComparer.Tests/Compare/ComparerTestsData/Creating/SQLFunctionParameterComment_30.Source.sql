set term ^;

create or alter function new_function (i int) returns int
as
begin
end^

set term ;^

comment on parameter new_function.i is 'test';