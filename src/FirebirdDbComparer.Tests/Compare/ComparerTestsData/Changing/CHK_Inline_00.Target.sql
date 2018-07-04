create table t (
    col_change int check (col_change > 6),
    col_drop int check (col_drop > 0),
    col_create int);