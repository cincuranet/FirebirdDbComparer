create table t (a int, b int);
alter table t add constraint unq_t unique (a) using index unq_t;