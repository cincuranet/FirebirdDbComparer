using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class DomainDataTypeUsedInUQAndFK_00 : ComparerTests.ITestCaseScriptSpecificAsserts
    {
        public void Execute(ScriptResult compareResult)
        {
            var commands = compareResult.AllStatements.ToArray();
            var dropConstraintCommands = commands.Where(x => x.Contains(" DROP CONSTRAINT ")).Count();
            var addUniqueCommands = commands.Where(x => x.Contains(" ADD UNIQUE ")).Count();
            var addForeignKeyCommands = commands.Where(x => x.Contains(" ADD FOREIGN KEY ")).Count();
            Assert.That(dropConstraintCommands, Is.EqualTo(2));
            Assert.That(addUniqueCommands, Is.EqualTo(1));
            Assert.That(addForeignKeyCommands, Is.EqualTo(1));
            Assert.That(commands.Count(), Is.EqualTo(5));
        }
    }
}
