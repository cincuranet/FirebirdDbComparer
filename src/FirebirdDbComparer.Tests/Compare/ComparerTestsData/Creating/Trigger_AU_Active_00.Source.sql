create table t (i int);

set term ^;

create trigger trig for t
active
after update
as
begin
end^

set term ;^