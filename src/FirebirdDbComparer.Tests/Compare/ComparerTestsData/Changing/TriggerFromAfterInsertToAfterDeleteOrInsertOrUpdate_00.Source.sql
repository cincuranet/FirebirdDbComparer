create table t (i int);

set term ^;

create trigger trig for t
after delete or insert or update
as
begin
end^

set term ;^