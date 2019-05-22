using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class ColumnToTableWithoutPosition_00 : ComparerTests.TestCaseSpecificAsserts
    {
        public override void AssertScript(ScriptResult compareResult)
        {
            base.AssertScript(compareResult);
            var commands = compareResult.AllStatements.ToArray();
            Assert.That(commands.Count(), Is.EqualTo(1));
        }
    }
}
