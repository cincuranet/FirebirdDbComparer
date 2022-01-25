using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing
{
    public class ViewAndTableWithCycleTriggers : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
				
";

        public override string Target => @"
create table t (a numeric(8,2));

create view v as select * from t;

set term ^;

create trigger trig_t for t
after delete
as
declare var numeric(8,2);
begin
    select first 1 a from v into var;
end^

create trigger trig_v for v
after delete
as
declare var numeric(8,2);
begin
    select first 1 a from t into var;
end^

set term ;^
";
    }
}
