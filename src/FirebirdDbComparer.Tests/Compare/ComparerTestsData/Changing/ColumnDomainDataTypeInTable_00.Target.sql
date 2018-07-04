create domain d_small as smallint;
create domain d_big as bigint;
create domain d_small_char as char(1);
create domain d_small_varchar as varchar(1);
create domain d_big_char as char(2);
create domain d_big_varchar as varchar(2);
create table t (a d_small, b d_small_char, c d_small_varchar);