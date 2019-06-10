create function new_ee_function(in1 integer, in2 bigint)
returns varchar(20)
external name 'FooBar!new_ee_function'
engine FbNetExternalEngine;