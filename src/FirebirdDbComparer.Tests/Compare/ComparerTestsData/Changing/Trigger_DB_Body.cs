using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class Trigger_DB_Body : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
set term ^;

create trigger trig
on transaction commit
as
begin
    -- foo
end^

set term ;^				
";

        public override string Target => @"
set term ^;

create trigger trig
on transaction commit
as
begin
    -- foobar
end^

set term ;^
";
    }
}
