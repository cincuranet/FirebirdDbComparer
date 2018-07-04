create table t (i int);
alter table t add constraint chk_t check (i > 0);