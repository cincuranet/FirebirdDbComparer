create domain d1 as char(1) collate unicode_ci_ai;
create domain d2 as varchar(1) collate unicode_ci_ai;

create table t (a d1, b d2);