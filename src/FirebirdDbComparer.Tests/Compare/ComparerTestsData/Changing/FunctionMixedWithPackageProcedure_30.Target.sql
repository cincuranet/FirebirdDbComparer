set term ^ ;

create function p (in_i int)
returns int
as
begin
    return in_i * 2;
end^

create package pkg
as
begin
    function p (in_i int) returns int;
end^
create package body pkg
as
begin
    function p (in_i int)
    returns int
    as
    begin
        return in_i * 2;
    end
end^

set term ;^