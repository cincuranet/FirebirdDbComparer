namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class ViewWithTriggers : ComparerTests.TestCaseStructure
{
    public override string Source => @"
				
";

    public override string Target => @"
create table mytable (
  a int primary key,
  b int default 0);

create view myview
as
select x.a, x.b + 1 c
  from mytable x;

set term ^;

create trigger myview_bd0 for myview
active before delete position 0
as
begin
  delete
    from mytable x
   where x.a = old.a;
end^

create trigger myview_bi0 for myview
active before insert position 0
as
begin
  insert into mytable(a, b)
  values (new.a, new.c - 1);
end^

create trigger myview_bu0 for myview
active before update position 0
as
begin
  update mytable x
     set x.b = new.c -1
   where x.a = old.a;
end^

set term ;^
";
}
