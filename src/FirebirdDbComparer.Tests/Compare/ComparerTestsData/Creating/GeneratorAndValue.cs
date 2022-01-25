using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class GeneratorAndValue : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create sequence new_generator;
alter sequence new_generator restart with 20;				
";

        public override string Target => @"

";
    }
}
