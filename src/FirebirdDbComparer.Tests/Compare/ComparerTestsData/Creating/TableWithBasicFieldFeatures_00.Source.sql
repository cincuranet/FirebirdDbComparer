create domain datetime as timestamp;
create domain intnn as int not null;

create table simple_table (
    i int,
    j varchar(20) character set utf8,
    k varchar(20) character set octets,
    l time not null,
    a decimal(18,4) not null,
    b datetime,
    c intnn,
    d datetime not null,
    e intnn not null);