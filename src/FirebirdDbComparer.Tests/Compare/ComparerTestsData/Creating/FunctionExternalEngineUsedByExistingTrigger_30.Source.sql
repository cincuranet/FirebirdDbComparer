create table t (i int);

create function new_ee_function(in1 integer)
returns integer
external name 'FooBar!new_ee_function'
engine FbNetExternalEngine;

set term ~;
create trigger trig for t
before insert
as
begin
    new.i = new_ee_function(new.i);
end~
set term ;~