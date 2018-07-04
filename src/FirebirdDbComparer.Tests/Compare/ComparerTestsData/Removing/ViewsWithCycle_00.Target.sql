create table root (i int);

create view v1
as
select i from root;

create view v2
as
select i from v1;

alter view v1
as
select i from v2;