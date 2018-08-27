set term ^;

create function foo(i int) returns int
as
begin
  return 1;
end^

create package some_pkg
as
begin
  function test(i int) returns int;
end^

create package body some_pkg
as
begin
  function test(i int) returns int
  as
  begin
    return foo(i);
  end
end^

set term ;^