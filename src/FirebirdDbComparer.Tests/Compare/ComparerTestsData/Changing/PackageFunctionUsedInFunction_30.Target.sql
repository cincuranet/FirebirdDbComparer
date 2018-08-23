set term ^;

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
    return 1;
  end
end^

create function foo returns int
as
begin
  return some_pkg.test(1);
end^

set term ;^