create domain number as int;
create table t (
  i int primary key,
  a number unique);
create table u (
  i int primary key,
  r number references t(a));
