create domain number as smallint;
create view v
as
select cast(null as number) as n from rdb$database;