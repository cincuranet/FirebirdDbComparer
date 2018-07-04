create table t (i int);

set term ^;

create trigger trig for t
after update
as
begin
end^

set term ;^