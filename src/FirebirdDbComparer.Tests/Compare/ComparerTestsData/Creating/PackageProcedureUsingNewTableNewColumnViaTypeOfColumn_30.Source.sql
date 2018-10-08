create table test (c bigint);

set term ^;

create package some_pkg
as
begin
  procedure test(i type of column test.c);
end^

set term ;^