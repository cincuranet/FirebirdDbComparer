using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class DomainDataTypeUsedInPK : ComparerTests.TestCaseStructure
    {
        public override void AssertScript(ScriptResult compareResult)
        {
            var commands = compareResult.AllStatements.ToArray();
            var dropConstraintCommands = commands.Where(x => x.Contains(" DROP CONSTRAINT ")).Count();
            var addPrimaryKeyCommands = commands.Where(x => x.Contains(" ADD PRIMARY KEY ")).Count();
            Assert.That(dropConstraintCommands, Is.EqualTo(1));
            Assert.That(addPrimaryKeyCommands, Is.EqualTo(1));
            Assert.That(commands.Count(), Is.EqualTo(3));
        }

        public override string Source => @"
create domain number as int;
create table t (i number primary key);				
";

        public override string Target => @"
create domain number as smallint;
create table t (i number primary key);
";
    }
}
