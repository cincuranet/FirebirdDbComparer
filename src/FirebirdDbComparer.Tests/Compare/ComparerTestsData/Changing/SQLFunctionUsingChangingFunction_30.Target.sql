set term ^;

create function testA(i int) returns int
as
begin
  return i + 1;
end^

create function testB(i int) returns int
as
begin
  return i + testA(i);
end^

set term ;^