using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class Trigger_AD_Inactive : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create table t (i int);

set term ^;

create trigger trig for t
inactive
after delete
as
begin
end^

set term ;^				
";

        public override string Target => @"

";
    }
}
