create function new_ee_function(in1 integer)
returns integer
external name 'FooBar!new_ee_function'
engine FbNetExternalEngine;