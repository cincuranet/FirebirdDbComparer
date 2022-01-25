using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class TriggerFromBeforeInsertToBeforeInsertOrDelete : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create table t (i int);

set term ^;

create trigger trig for t
before insert or delete
as
begin
end^

set term ;^				
";

        public override string Target => @"
create table t (i int);

set term ^;

create trigger trig for t
before insert
as
begin
end^

set term ;^
";
    }
}
