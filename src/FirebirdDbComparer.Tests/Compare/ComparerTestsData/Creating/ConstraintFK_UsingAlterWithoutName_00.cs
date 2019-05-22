using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class ConstraintFK_UsingAlterWithoutName_00 : ComparerTests.TestCaseSpecificAsserts
    {
        public override void AssertScript(ScriptResult compareResult)
        {
            base.AssertScript(compareResult);
            var commands = compareResult.AllStatements
                .Where(c => c.Contains(" CONSTRAINT "))
                .ToArray();
            Assert.That(commands, Is.Empty);
        }
    }
}

