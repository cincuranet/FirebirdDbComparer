create table t (i int);

create desc index idx
on t computed by (i*2);