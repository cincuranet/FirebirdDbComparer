set term ^;

create package some_pkg
as
begin
  procedure test(i bigint);
end^

create package body some_pkg
as
begin
  procedure test(i bigint)
  as
  begin
  end
end^

create procedure foo(i bigint)
as
begin
  execute procedure some_pkg.test(i);
end^

set term ;^