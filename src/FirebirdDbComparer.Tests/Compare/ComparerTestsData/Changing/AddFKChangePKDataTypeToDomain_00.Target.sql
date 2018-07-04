create domain d_primary bigint not null;
create domain d_foreign_nn bigint not null;
create table parent (id bigint not null);
create table child (id bigint not null, id_parent bigint not null);
alter table parent add constraint pk_parent primary key (id);
alter table child add constraint pk_child primary key (id);