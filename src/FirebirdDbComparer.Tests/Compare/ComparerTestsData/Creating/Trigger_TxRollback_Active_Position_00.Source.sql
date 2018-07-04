set term ^;

create trigger trig
active
on transaction rollback
position 666
as
begin
end^

set term ;^