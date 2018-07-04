create table t(i int);

set term ^;

create function test(i int) returns boolean
as
begin
  return true;
end^

create trigger trig for t
before insert
as
begin
  if (test(new.i)) then
  begin
  end
end^

set term ;^