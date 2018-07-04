create table t (i int);

set term ^;

create trigger trig for t
before insert or update
as
begin
end^

set term ;^