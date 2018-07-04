create view t(i)
as
select 1 as i from rdb$database;

create table t_test (
    i  integer,
    j  computed by ((select first 1 i from t))
);

create view v_test(i)
as
select * from t;
