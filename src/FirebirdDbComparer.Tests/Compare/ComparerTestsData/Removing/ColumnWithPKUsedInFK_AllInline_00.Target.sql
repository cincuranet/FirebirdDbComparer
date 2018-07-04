create table t_master (i int, a int not null primary key);
create table t_detail (a int not null primary key, drop_me int references t_master(a));
