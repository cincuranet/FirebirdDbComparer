set term ^;

create package some_pkg
as
begin
  procedure test(i int);
end^

create package body some_pkg
as
begin
  procedure test(i int)
  as
  begin
  end

  procedure change(i bigint)
  as
  begin
  end
end^

set term ;^