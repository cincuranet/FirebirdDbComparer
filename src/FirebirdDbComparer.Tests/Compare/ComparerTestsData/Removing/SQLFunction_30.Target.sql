set term ^;

create function new_function(i int) returns int
as
begin
 return i + 1;
end^

set term ;^