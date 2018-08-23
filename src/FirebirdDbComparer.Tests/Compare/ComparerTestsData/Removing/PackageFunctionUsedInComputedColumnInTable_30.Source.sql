set term ^;

create package some_pkg
as
begin
  function b(i int) returns int;
end^

create package body some_pkg
as
begin
  function b(i int) returns int
  as
  begin
    return 1;
  end
end^

set term ;^

create table test (i int, c computed by (some_pkg.b(i)));