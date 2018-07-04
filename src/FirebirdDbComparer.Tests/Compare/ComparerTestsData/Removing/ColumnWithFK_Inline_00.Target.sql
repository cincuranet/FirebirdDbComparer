create table t_master (a int primary key);
create table t_detail (a int primary key, drop_me int references t_master(a));
