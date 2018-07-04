declare external function test
  cstring(100)
returns integer by value
entry_point 'Test' module_name 'Test';

set term ^;

create or alter trigger dbdisconnect
active on disconnect
as
declare var int;
begin
  var = test('test');
end^

set term ;^