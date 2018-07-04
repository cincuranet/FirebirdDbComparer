create view v
as
select 1 as i from rdb$database;

set term ^ ;

create procedure p
returns (out_i int)
as
begin
    for select i from v into out_i do
    begin
        suspend;
    end
end^

set term ;^