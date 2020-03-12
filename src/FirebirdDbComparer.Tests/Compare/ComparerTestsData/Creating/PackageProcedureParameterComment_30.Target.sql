set term ^;

create package some_pkg
as
begin
  procedure test(i int) returns (j int);
end^

create package body some_pkg
as
begin
  procedure test(i int) returns (j int)
  as
  begin
  end
end^

set term ;^