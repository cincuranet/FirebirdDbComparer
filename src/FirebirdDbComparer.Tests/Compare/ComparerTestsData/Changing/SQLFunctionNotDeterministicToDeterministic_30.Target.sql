set term ^;

create function test(i int) returns int
as
begin
  return i;
end^

set term ;^