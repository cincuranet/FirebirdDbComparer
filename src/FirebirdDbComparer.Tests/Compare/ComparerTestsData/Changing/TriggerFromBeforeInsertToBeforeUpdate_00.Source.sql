create table t (i int);

set term ^;

create trigger trig for t
before update
as
begin
end^

set term ;^