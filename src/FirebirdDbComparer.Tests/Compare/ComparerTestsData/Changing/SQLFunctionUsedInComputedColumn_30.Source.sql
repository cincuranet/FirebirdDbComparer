set term ^;

create function test(i int, j int) returns int
as
begin
  return i + 2;
end^

set term ;^

create table t(i int, j computed by (test(i, i)));