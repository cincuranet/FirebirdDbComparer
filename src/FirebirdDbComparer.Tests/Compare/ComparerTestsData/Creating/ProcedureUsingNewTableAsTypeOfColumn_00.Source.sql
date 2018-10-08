create table t (i int);

set term ^;

create procedure p (i type of column t.i)
as
begin
end^

set term ;^