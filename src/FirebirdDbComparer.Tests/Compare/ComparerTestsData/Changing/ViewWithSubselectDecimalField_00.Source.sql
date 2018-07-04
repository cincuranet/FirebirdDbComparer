create view v
as
select 1 as i, cast((select null from rdb$database) as decimal(12,3)) as j from rdb$database;