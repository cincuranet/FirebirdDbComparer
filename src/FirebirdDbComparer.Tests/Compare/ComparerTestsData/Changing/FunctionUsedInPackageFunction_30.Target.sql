﻿set term ^;

create function foo(i int) returns int
as
begin
  return 1;
end^

create package some_pkg
as
begin
  function test returns int;
end^

create package body some_pkg
as
begin
  function test returns int
  as
  begin
    return foo(1);
  end
end^

set term ;^
