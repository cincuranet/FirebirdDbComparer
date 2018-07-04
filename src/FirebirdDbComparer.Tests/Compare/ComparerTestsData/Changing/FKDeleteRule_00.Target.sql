create table t1 (a int primary key);
create table t2 (a int, b int);
alter table t2 add constraint fk_test foreign key (b) references t1(a);