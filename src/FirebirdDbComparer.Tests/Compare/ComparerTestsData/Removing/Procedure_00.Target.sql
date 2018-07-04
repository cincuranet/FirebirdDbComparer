create domain d_test as int;

set term ^;

create procedure new_procedure (
    in1 varchar(10) character set utf8 collate ucs_basic,
    in2 d_test,
    in3 type of d_test)
returns (
    out1 integer,
    out2 d_test,
    out3 type of d_test)
as
declare var d_test;
begin
end^

set term ;^
