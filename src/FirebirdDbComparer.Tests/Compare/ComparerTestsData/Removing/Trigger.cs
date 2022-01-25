using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing
{
    public class Trigger : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create table t (i int);				
";

        public override string Target => @"
create table t (i int);

set term ^;

create trigger trig for t
after delete
as
begin
end^

set term ;^
";
    }
}
