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
end^

create procedure foo(i int)
as
begin
  execute procedure some_pkg.test(i);
end^

set term ;^