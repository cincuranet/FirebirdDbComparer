create table t (i int);
alter table t add check (i > 1);
alter table t add check (i > 2);