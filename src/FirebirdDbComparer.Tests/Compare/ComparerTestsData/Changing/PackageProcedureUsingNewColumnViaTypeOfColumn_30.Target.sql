create table test (x int);

set term ^;

create package some_pkg
as
begin
  procedure test(i int);
end^

set term ;^