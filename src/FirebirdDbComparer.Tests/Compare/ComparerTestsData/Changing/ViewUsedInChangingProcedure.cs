using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class ViewUsedInChangingProcedure : ComparerTests.TestCaseStructure
{
    public override string Source => @"
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
";

    public override string Target => @"
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
";
}
