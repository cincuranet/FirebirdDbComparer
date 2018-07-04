create domain d_string as varchar(20) character set utf8;
create table t (a d_string, b char(20));