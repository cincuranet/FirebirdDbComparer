set term ^;

create package some_pkg
as
begin
  function test(i bigint) returns bigint;
end^

create package body some_pkg
as
begin
  function test(i bigint) returns bigint
  as
  begin
    return 1;
  end
end^

set term ;^