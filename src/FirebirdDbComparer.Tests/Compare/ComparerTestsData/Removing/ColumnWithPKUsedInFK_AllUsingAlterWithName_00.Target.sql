create table t_master (i int, a int not null);
alter table t_master add constraint pk_master primary key (a);
create table t_detail (a int not null, drop_me int);
alter table t_detail add constraint pk_detail primary key (a);
alter table t_detail add constraint fk_detail_master foreign key (drop_me) references t_master(a);
