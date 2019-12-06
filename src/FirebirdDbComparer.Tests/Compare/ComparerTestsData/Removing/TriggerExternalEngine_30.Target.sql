create table t (id int);

create trigger new_ee_trigger
after update on t
external name 'FooBar!new_ee_trigger'
engine FbNetExternalEngine;