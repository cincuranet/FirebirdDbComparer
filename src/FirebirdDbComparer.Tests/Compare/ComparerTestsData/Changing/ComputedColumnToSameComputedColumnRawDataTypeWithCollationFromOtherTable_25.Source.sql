create table parent (
    id bigint not null,
    code varchar(512) character set utf8 not null collate unicode_ci
);
create table foobar (
    id bigint not null,
    id_master bigint not null,
    code computed by ((select code from parent where id = id_master))
);