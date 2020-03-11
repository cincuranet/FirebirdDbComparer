set term ^ ;

create function p (in_i int)
returns int
as
begin
    return in_i * 2;
end^

set term ;^