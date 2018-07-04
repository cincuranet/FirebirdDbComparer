create table t (i int);

set term ^;

create trigger trig for t
before update position 6
as
begin
end^

set term ;^