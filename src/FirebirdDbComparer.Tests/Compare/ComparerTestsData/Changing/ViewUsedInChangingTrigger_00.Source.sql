create view v
as
select 2 as i from rdb$database;

create table t (i int);

set term ^;

create trigger trig for t
before insert
as
declare i int;
begin
    select first 1 i*2 from v into i;
end^

set term ;^