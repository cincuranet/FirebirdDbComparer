create table t (i int);

set term ^;

create trigger trig for t
before update position 66
as
begin
end^

set term ;^