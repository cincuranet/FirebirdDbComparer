create table master (id int unique);
create table detail (id int primary key, fk int references master(id));