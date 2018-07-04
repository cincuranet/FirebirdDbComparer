using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class CHKsMultiple_AlterWithoutName_00 : ComparerTests.ITestCaseScriptSpecificAsserts
    {
        public void Execute(ScriptResult compareResult)
        {
            var commands = compareResult.AllStatements.ToArray();
            var dropCommands = commands.Where(x => x.Contains(" DROP ")).Count();
            var addCommands = commands.Where(x => x.Contains(" ADD ")).Count();
            Assert.That(dropCommands, Is.EqualTo(3));
            Assert.That(addCommands, Is.EqualTo(1));
        }
    }
}
