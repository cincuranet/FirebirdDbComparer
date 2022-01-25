using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class ViewWithSubselectDecimalField : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create view v
as
select 1 as i, cast((select null from rdb$database) as decimal(12,3)) as j from rdb$database;				
";

        public override string Target => @"
create view v
as
select 1 as i from rdb$database;
";
    }
}
