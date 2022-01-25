using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing
{
    public class TablesWithCycleTriggers : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
				
";

        public override string Target => @"
create table t1 (a numeric(8,2));
create table t2 (a numeric(8,2));

set term ^;

create trigger trig_t1 for t1
after delete
as
declare var numeric(8,2);
begin
    select first 1 a from t2 into var;
end^

create trigger trig_t2 for t2
after delete
as
declare var numeric(8,2);
begin
    select first 1 a from t1 into var;
end^

set term ;^
";
    }
}
