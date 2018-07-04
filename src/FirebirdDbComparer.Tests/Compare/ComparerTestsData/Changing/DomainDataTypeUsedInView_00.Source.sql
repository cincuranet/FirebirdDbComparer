create domain number as int;
create view v
as
select cast(null as number) as n from rdb$database;