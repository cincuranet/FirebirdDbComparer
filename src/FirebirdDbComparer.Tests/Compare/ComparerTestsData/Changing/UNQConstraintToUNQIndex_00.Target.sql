create table t (i int);
alter table t add constraint unq_i unique(i) using index unq_i;