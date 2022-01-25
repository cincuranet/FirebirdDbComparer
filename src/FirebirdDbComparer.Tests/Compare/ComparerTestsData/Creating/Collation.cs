using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class Collation : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create collation iso8859_1_unicode for iso8859_1;				
";

        public override string Target => @"

";
    }
}
