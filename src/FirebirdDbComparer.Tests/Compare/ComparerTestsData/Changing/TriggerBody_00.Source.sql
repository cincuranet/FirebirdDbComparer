create table t (i int);

set term ^;

create trigger trig for t
before update
as
begin
    new.i = old.i;
end^

set term ;^