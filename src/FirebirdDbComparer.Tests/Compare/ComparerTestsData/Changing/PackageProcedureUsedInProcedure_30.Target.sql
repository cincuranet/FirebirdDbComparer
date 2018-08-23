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

create procedure foo
as
begin
  execute procedure some_pkg.test(1);
end^

set term ;^