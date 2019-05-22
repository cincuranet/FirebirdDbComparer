set term ^;

create trigger trig
on transaction commit
as
begin
    -- foo
end^

set term ;^