set term ^;

create package some_pkg
as
begin
  function test(i int) returns bigint;
end^

create package body some_pkg
as
begin
  function test(i int) returns bigint
  as
  begin
    return 1;
  end
end^

set term ;^

create table test (i int, c computed by (some_pkg.test(i)));;