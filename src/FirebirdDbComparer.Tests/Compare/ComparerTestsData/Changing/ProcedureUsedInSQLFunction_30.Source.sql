set term ^;

create procedure test_procedure
returns (i bigint)
as
begin
  i = 2;
  suspend;
end^

create function call_proc() returns bigint
as
declare i bigint;
begin
  select sum(i) from test_procedure into i;
  return i;
end^

set term ;^