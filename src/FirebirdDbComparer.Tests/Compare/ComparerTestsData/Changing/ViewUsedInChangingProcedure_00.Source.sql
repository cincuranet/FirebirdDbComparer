create view v
as
select 2 as i from rdb$database;

set term ^ ;

create procedure p
returns (out_i int)
as
begin
    for select i * 2 from v into out_i do
    begin
        suspend;
    end
end^

set term ;^