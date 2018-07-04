declare external function test
  cstring(200)
returns integer by value
entry_point 'Test2' module_name 'Test2';

set term ^;

create or alter trigger dbdisconnect
active on disconnect
as
declare var int;
begin
  var = test('test');
end^

set term ;^