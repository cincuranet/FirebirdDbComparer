using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing
{
    public class ColumnWithPKUsedInFK_AllUsingAlterWithName_00 : ComparerTests.ITestCaseScriptSpecificAsserts
    {
        public void Execute(ScriptResult compareResult)
        {
            var commands = compareResult.AllStatements.ToArray();
            Assert.That(commands.Count(), Is.EqualTo(3));
        }
    }
}
