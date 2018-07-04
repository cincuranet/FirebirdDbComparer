create table t (
    col_change int check (col_change > 66),
    col_drop int,
    col_create int check (col_create > 0));