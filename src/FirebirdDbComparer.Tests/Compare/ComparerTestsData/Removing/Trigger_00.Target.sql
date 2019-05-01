create table t (i int);

set term ^;

create trigger trig for t
after delete
as
begin
end^

set term ;^