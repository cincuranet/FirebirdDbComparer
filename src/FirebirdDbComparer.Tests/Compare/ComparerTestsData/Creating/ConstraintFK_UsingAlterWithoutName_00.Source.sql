create table t_master (i int primary key);
create table t_detail (i int primary key);
alter table t_detail add foreign key (i) references t_master(i);