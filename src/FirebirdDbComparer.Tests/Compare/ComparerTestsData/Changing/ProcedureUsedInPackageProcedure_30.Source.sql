set term ^;

create procedure foo(i bigint)
as
begin
end^

create package some_pkg
as
begin
  procedure test;
end^

create package body some_pkg
as
begin
  procedure test
  as
  begin
    execute procedure foo(1);
  end
end^

set term ;^
