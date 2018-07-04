create table t (i int);

set term ^;

create trigger trig for t
inactive
before update
as
begin
end^

set term ;^