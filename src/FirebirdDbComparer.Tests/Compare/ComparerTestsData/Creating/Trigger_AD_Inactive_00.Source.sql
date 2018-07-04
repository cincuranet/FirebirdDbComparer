create table t (i int);

set term ^;

create trigger trig for t
inactive
after delete
as
begin
end^

set term ;^