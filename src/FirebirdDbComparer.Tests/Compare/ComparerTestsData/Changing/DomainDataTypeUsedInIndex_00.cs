using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class DomainDataTypeUsedInIndex_00 : ComparerTests.TestCaseSpecificAsserts
    {
        public override void AssertScript(ScriptResult compareResult)
        {
            base.AssertScript(compareResult);
            var commands = compareResult.AllStatements.ToArray();
            var indexCommands = commands.Where(x => x.Contains(" INDEX ")).Count();
            Assert.That(indexCommands, Is.EqualTo(2));
            Assert.That(commands.Count(), Is.EqualTo(3));
        }
    }
}
