using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class DomainDataTypeUsedInUQ : ComparerTests.TestCaseStructure
{
    public override void AssertScript(ScriptResult compareResult)
    {
        var commands = compareResult.AllStatements.ToArray();
        var dropConstraintCommands = commands.Where(x => x.Contains(" DROP CONSTRAINT ")).Count();
        var addUniqueCommands = commands.Where(x => x.Contains(" ADD UNIQUE ")).Count();
        Assert.That(dropConstraintCommands, Is.EqualTo(1));
        Assert.That(addUniqueCommands, Is.EqualTo(1));
        Assert.That(commands.Count(), Is.EqualTo(3));
    }

    public override string Source => @"
create domain number as int;
create table t (
  i int primary key,
  a number unique);				
";

    public override string Target => @"
create domain number as smallint;
create table t (
  i int primary key,
  a number unique);

";
}
