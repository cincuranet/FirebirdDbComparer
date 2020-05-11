create table t (i int);

set term ~;
create trigger trig for t
before insert
as
begin
end~
set term ;~