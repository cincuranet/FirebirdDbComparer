create table t (i int);

set term ^;

create trigger trig for t
active
after insert or update or delete
position 222
as
begin
end^

set term ;^