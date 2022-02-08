using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class DomainDataTypeUsedInUQAndFK : ComparerTests.TestCaseStructure
{
    public override void AssertScript(ScriptResult compareResult)
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

    public override string Source => @"
create domain number as int;
create table t (
  i int primary key,
  a number unique);
create table u (
  i int primary key,
  r number references t(a));
				
";

    public override string Target => @"
create domain number as smallint;
create table t (
  i int primary key,
  a number unique);
create table u (
  i int primary key,
  r number references t(a));

";
}
