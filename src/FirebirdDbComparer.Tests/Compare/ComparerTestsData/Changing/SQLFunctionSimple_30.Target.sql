set term ^;

create function test(i int) returns int
as
begin
  return i + 1;
end^

set term ;^