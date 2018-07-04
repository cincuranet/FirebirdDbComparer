set term ^;

create or alter function new_function (i int) returns int
as
begin
end^

set term ;^

comment on function new_function is 'test';