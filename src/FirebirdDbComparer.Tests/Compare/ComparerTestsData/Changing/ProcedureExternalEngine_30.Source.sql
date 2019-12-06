create procedure new_ee_procedure(in1 bigint)
returns (out1 bigint)
external name 'FooBar!new_ee_procedure'
engine FbNetExternalEngine;