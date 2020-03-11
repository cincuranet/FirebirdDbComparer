set term ^ ;

create procedure p (in_i int)
returns (out_i int)
as
begin
    out_i = in_i * 2;
end^

create package pkg
as
begin
    procedure p (in_i int) returns (out_i int);
end^
create package body pkg
as
begin
    procedure p (in_i int)
    returns (out_i int)
    as
    begin
        out_i = in_i * 2;
    end
end^

set term ;^