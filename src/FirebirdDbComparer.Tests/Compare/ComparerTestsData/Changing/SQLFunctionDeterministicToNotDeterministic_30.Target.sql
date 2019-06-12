set term ^;

create function test(i int) returns int deterministic
as
begin
  return i;
end^

set term ;^