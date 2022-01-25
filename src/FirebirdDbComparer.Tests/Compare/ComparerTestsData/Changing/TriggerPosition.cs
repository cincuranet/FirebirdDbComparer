using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class TriggerPosition : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create table t (i int);

set term ^;

create trigger trig for t
before update position 6
as
begin
end^

set term ;^				
";

        public override string Target => @"
create table t (i int);

set term ^;

create trigger trig for t
before update position 66
as
begin
end^

set term ;^
";
    }
}
