create table master (id int primary key);
create table detail (id int primary key, fk int references master(id));