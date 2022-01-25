using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class SimpleViewWithCheck : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create or alter view test_view
as
select * from rdb$database
where 1=1
with check option
;				
";

        public override string Target => @"

";
    }
}
