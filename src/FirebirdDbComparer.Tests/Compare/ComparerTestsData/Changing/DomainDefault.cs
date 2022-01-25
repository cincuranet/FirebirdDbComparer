using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class DomainDefault : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create domain a as int;
create domain b as int default 2;				
";

        public override string Target => @"
create domain a as int default 1;
create domain b as int;
";
    }
}
