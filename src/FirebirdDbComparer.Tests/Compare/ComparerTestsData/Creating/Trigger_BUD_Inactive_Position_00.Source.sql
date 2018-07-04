create table t (i int);

set term ^;

create trigger trig for t
inactive
before update or delete
position 66
as
begin
end^

set term ;^