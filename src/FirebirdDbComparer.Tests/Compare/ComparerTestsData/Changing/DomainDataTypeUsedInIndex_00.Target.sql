create domain number as smallint;
create table t (i bigint primary key, a number);
create index ix_a on t(a);
