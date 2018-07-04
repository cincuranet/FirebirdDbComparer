create table t (i int);

set term ^;

create trigger trig for t
active
before delete
as
begin
end^

set term ;^