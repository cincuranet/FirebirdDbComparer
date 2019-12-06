create function new_ee_function(in1 bigint)
returns bigint
external name 'FooBar!new_ee_function'
engine FbNetExternalEngine;