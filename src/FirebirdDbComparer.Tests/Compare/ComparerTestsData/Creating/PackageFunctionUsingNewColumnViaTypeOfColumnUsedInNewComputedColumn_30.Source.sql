create table test (x bigint, t smallint);

set term ^;

create package some_pkg
as
begin
  function test(x type of column test.t) returns type of column test.t;
end^

create package body some_pkg
as
begin
  function test(x type of column test.t) returns type of column test.t
  as
  begin
    return x;
  end
end^

set term ;^

alter table test add c computed by (some_pkg.test(t));