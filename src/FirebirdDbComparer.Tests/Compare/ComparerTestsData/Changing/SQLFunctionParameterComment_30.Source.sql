set term ^;

create or alter function new_function (i int) returns int
as
begin
    return i;
end^

set term ;^

comment on parameter new_function.i is 'test_new';