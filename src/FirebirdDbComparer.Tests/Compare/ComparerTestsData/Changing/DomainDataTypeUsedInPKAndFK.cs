using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class DomainDataTypeUsedInPKAndFK : ComparerTests.TestCaseStructure
    {
        public override void AssertScript(ScriptResult compareResult)
        {
            var commands = compareResult.AllStatements.ToArray();
            var dropConstraintCommands = commands.Where(x => x.Contains(" DROP CONSTRAINT ")).Count();
            var addPrimaryKeyCommands = commands.Where(x => x.Contains(" ADD PRIMARY KEY ")).Count();
            var addForeignKeyCommands = commands.Where(x => x.Contains(" ADD FOREIGN KEY ")).Count();
            Assert.That(dropConstraintCommands, Is.EqualTo(5));
            Assert.That(addPrimaryKeyCommands, Is.EqualTo(3));
            Assert.That(addForeignKeyCommands, Is.EqualTo(2));
            Assert.That(commands.Count(), Is.EqualTo(11));
        }

        public override string Source => @"
create domain number as int;
create table t (i number primary key);
create table u (
  i number primary key,
  r number references t);
create table v (
  i number primary key references t);
				
";

        public override string Target => @"
create domain number as smallint;
create table t (i number primary key);
create table u (
  i number primary key,
  r number references t);
create table v (
  i number primary key references t);

";
    }
}
