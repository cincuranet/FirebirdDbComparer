using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class ConstraintPK_Inline : ComparerTests.TestCaseStructure
    {
        public override void AssertScript(ScriptResult compareResult)
        {
            var commands = compareResult.AllStatements
                .Where(c => c.Contains(" CONSTRAINT "))
                .ToArray();
            Assert.That(commands, Is.Empty);
        }

        public override string Source => @"
create table t (i int not null primary key);				
";

        public override string Target => @"

";
    }
}
