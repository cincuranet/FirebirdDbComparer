set term ^;

create trigger trig
active
after create package or drop function or create function or drop exception or alter role or drop mapping
as
begin
end^

set term ;^