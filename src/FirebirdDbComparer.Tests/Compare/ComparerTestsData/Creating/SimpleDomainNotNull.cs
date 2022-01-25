using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class SimpleDomainNotNull : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create domain d_test as int not null;				
";

        public override string Target => @"

";
    }
}
