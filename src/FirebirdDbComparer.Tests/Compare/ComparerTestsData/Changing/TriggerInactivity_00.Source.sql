create table t (i int);

set term ^;

create trigger trig1 for t
inactive
before update
as
begin
end^

create trigger trig2 for t
before update
as
begin
end^

set term ;^