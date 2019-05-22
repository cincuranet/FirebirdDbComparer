set term ^;

create trigger trig
on transaction commit
as
begin
    -- foobar
end^

set term ;^