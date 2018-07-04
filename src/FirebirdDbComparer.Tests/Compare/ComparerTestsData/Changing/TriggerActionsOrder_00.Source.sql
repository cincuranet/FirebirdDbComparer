create table t (i int);

set term ^;

create trigger trig for t
before update or insert
as
begin
end^

set term ;^