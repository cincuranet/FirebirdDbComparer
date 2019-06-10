create procedure new_ee_procedure(in1 integer)
returns (out1 integer)
external name 'FooBar!new_ee_procedure'
engine FbNetExternalEngine;