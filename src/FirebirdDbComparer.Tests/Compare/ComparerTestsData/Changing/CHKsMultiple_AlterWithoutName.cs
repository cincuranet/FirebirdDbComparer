using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class CHKsMultiple_AlterWithoutName : ComparerTests.TestCaseStructure
{
    public override void AssertScript(ScriptResult compareResult)
    {
        var commands = compareResult.AllStatements.ToArray();
        var dropCommands = commands.Where(x => x.Contains(" DROP ")).Count();
        var addCommands = commands.Where(x => x.Contains(" ADD ")).Count();
        Assert.That(dropCommands, Is.EqualTo(3));
        Assert.That(addCommands, Is.EqualTo(1));
    }

    public override string Source => @"
create table t (i int);
alter table t add check (i > 1);
alter table t add check (i > 2);
";

    public override string Target => @"
create table t (i int);
alter table t add check (i > 3);
alter table t add check (i > 1);
alter table t add check (i > 1);
alter table t add check (i > 1);
";
}
