create table t (a int);
alter table t add b computed by (1);
alter table t add c computed by (b+b);
alter table t alter b computed by (a+a);
