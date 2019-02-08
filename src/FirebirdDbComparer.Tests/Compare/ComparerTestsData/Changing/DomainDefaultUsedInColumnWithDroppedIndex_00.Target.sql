create domain d_test int default 10;
create table t(col d_test);
create index idx_t_col on t(col);