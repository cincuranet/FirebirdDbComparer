set term ^;

create function test(i bigint) returns bigint
as
begin
  return i + 2;
end^

create procedure call_func
returns (i bigint)
as
begin
  i = test(2);
  suspend;
end^

set term ;^