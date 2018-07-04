create table t (
    i int,
    j computed by (i * 1));
alter table t add constraint chk_j check (j > 0);