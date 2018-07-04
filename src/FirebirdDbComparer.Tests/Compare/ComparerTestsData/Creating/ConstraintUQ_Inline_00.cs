using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class ConstraintUQ_Inline_00 : ComparerTests.ITestCaseScriptSpecificAsserts
    {
        public void Execute(ScriptResult compareResult)
        {
            var commands = compareResult.AllStatements
                .Where(c => c.Contains(" CONSTRAINT "))
                .ToArray();
            Assert.That(commands, Is.Empty);
        }
    }
}
