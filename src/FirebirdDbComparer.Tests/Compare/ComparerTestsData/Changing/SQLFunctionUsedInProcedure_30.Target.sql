set term ^;

create function test(i int) returns int
as
begin
  return i + 1;
end^

create procedure call_func
returns (i int)
as
begin
  i = test(1);
  suspend;
end^

set term ;^