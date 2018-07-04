create table t (i int, j date);

create unique asc index idx
on t(i, j);