create table t (i int);

create procedure new_ee_procedure(in1 integer)
returns (out1 integer)
external name 'FooBar!new_ee_procedure'
engine FbNetExternalEngine;

set term ~;
create trigger trig for t
before insert
as
begin
    new.i = (select out1 from new_ee_procedure(new.i));
end~
set term ;~