create table t (
    i  int
);

create table t_test (
    i  int,
    j  computed by ((select first 1 i from t))
);

create view v_test(i)
as
select * from t;
