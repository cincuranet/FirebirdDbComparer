set term ^;

create procedure test_procedure
returns (i int)
as
begin
  i = 1;
  suspend;
end^

create function call_proc() returns int
as
declare i int;
begin
  select sum(i) from test_procedure into i;
  return i;
end^

set term ;^