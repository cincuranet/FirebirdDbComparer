using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class DomainNullable : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create domain a as int;
create domain b as int not null;				
";

        public override string Target => @"
create domain a as int not null;
create domain b as int;
";
    }
}
