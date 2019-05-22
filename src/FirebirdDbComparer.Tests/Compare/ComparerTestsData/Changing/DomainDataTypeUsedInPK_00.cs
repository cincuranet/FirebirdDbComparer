using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class DomainDataTypeUsedInPK_00 : ComparerTests.TestCaseSpecificAsserts
    {
        public override void AssertScript(ScriptResult compareResult)
        {
            base.AssertScript(compareResult);
            var commands = compareResult.AllStatements.ToArray();
            var dropConstraintCommands = commands.Where(x => x.Contains(" DROP CONSTRAINT ")).Count();
            var addPrimaryKeyCommands = commands.Where(x => x.Contains(" ADD PRIMARY KEY ")).Count();
            Assert.That(dropConstraintCommands, Is.EqualTo(1));
            Assert.That(addPrimaryKeyCommands, Is.EqualTo(1));
            Assert.That(commands.Count(), Is.EqualTo(3));
        }
    }
}
