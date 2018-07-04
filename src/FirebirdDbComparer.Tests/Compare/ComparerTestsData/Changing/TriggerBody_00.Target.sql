create table t (i int);

set term ^;

create trigger trig for t
before update
as
begin
    new.i = old.i * 2;
end^

set term ;^