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
        return i;
    end
end^

set term ;^

comment on parameter some_pkg.test.i is 'pkg';