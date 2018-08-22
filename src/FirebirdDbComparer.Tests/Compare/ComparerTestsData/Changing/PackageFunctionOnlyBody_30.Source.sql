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
  end

  function change(i bigint) returns bigint
  as
  begin
    return 2;
  end
end^

set term ;^