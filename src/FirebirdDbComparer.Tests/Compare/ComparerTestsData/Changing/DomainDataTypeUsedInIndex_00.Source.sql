create domain number as int;
create table t (i bigint primary key, a number);
create index ix_a on t(a);
