using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class TriggerInactivity : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create table t (i int);

set term ^;

create trigger trig1 for t
inactive
before update
as
begin
end^

create trigger trig2 for t
before update
as
begin
end^

set term ;^				
";

        public override string Target => @"
create table t (i int);

set term ^;

create trigger trig1 for t
before update
as
begin
end^

create trigger trig2 for t
inactive
before update
as
begin
end^

set term ;^
";
    }
}
