set term ^;

create package some_pkg
as
begin
  function test(i int) returns int;
end^

set term ;^