set term ^;

create procedure p1
as
begin
end^

create procedure p2
as
begin
    execute procedure p1;
end^

alter procedure p1
as
begin
    execute procedure p2;
end^

set term ;^
