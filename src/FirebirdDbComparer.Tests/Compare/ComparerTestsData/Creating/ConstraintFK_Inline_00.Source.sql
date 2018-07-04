create table t_master (i int primary key);
create table t_detail (i int primary key references t_master(i));