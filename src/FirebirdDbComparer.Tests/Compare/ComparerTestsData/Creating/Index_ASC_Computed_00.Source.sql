create table t (i int);

create asc index idx
on t computed by (i*2);