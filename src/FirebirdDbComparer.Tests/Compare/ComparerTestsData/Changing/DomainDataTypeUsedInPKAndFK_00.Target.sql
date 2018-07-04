create domain number as smallint;
create table t (i number primary key);
create table u (
  i number primary key,
  r number references t);
create table v (
  i number primary key references t);
