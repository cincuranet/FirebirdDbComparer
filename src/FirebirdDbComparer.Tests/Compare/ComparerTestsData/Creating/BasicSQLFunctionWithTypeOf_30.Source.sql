create domain d_test as varchar(20);

set term ^;

create or alter function new_function (i type of d_test) returns type of d_test
as
begin
end^

set term ;^