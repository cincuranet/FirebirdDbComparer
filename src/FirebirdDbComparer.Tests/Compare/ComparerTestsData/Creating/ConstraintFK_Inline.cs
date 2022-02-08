using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class ConstraintFK_Inline : ComparerTests.TestCaseStructure
{
    public override void AssertScript(ScriptResult compareResult)
    {
        var commands = compareResult.AllStatements
            .Where(c => c.Contains(" CONSTRAINT "))
            .ToArray();
        Assert.That(commands, Is.Empty);
    }

    public override string Source => @"
create table t_master (i int primary key);
create table t_detail (i int primary key references t_master(i));				
";

    public override string Target => @"

";
}

