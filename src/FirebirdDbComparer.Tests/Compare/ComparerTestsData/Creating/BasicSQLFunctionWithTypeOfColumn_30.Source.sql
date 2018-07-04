create table t (i int);

set term ^;

create or alter function new_function (i type of column t.i) returns type of column t.i
as
begin
end^

set term ;^