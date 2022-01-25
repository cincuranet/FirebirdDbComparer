using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class DomainDataTypeUsedInView : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create domain number as int;
create view v
as
select cast(null as number) as n from rdb$database;				
";

        public override string Target => @"
create domain number as smallint;
create view v
as
select cast(null as number) as n from rdb$database;
";
    }
}
