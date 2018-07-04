using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class DomainDataTypeUsedInUQ_00 : ComparerTests.ITestCaseScriptSpecificAsserts
    {
        public void Execute(ScriptResult compareResult)
        {
            var commands = compareResult.AllStatements.ToArray();
            var dropConstraintCommands = commands.Where(x => x.Contains(" DROP CONSTRAINT ")).Count();
            var addUniqueCommands = commands.Where(x => x.Contains(" ADD UNIQUE ")).Count();
            Assert.That(dropConstraintCommands, Is.EqualTo(1));
            Assert.That(addUniqueCommands, Is.EqualTo(1));
            Assert.That(commands.Count(), Is.EqualTo(3));
        }
    }
}
