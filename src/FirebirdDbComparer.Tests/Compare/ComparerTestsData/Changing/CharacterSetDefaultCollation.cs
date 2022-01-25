using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class CharacterSetDefaultCollation : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
alter character set utf8 set default collation unicode_ci;				
";

        public override string Target => @"

";
    }
}
