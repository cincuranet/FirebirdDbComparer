using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class DomainDataTypeUsedInIndex : ComparerTests.TestCaseStructure
{
    public override void AssertScript(ScriptResult compareResult)
    {
        var commands = compareResult.AllStatements.ToArray();
        var indexCommands = commands.Where(x => x.Contains(" INDEX ")).Count();
        Assert.That(indexCommands, Is.EqualTo(2));
        Assert.That(commands.Count(), Is.EqualTo(3));
    }

    public override string Source => @"
create domain number as int;
create table t (i bigint primary key, a number);
create index ix_a on t(a);
				
";

    public override string Target => @"
create domain number as smallint;
create table t (i bigint primary key, a number);
create index ix_a on t(a);

";
}
