using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class Trigger_BUD_Inactive_Position : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create table t (i int);

set term ^;

create trigger trig for t
inactive
before update or delete
position 66
as
begin
end^

set term ;^				
";

        public override string Target => @"

";
    }
}
