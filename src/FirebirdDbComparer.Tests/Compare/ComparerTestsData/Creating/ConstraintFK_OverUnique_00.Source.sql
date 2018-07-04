create table t_master (i int);
alter table t_master add unique (i);
create table t_detail (i int primary key);
alter table t_detail add constraint fk_detail_master foreign key (i) references t_master(i);