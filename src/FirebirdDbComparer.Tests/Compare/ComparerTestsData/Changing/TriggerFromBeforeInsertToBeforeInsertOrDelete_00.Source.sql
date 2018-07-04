create table t (i int);

set term ^;

create trigger trig for t
before insert or delete
as
begin
end^

set term ;^