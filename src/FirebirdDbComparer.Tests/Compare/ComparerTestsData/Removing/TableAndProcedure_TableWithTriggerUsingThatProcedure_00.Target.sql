create table t (a varchar(20));

set term ^;

create procedure p (in_a varchar(20))
returns (out_a varchar(20))
as
begin
    out_a = in_a;
end^

create trigger trig for t
before update
as
declare var_a varchar(20);
begin
    execute procedure p(new.a) returning_values (var_a);
    new.a = var_a;
end^

set term ;^