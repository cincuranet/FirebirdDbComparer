using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class SimpleException : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create exception new_exception 'text';				
";

        public override string Target => @"

";
    }
}
