create table t (a int);

set term ^;

create procedure p (in_i int)
returns (out_i int)
as
begin
    out_i = in_i;
    suspend;
end^

set term ;^