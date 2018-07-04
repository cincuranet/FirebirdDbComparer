create table t_master (i int);
create table t_detail (a int not null);
alter table t_detail add constraint pk_detail primary key (a);