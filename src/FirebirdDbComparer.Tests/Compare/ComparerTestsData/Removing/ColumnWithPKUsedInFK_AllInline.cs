using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class ColumnWithPKUsedInFK_AllInline : ComparerTests.TestCaseStructure
{
    public override void AssertScript(ScriptResult compareResult)
    {
        var commands = compareResult.AllStatements.ToArray();
        Assert.That(commands.Count(), Is.EqualTo(3));
    }

    public override string Source => @"
create table t_master (i int);
create table t_detail (a int not null primary key);				
";

    public override string Target => @"
create table t_master (i int, a int not null primary key);
create table t_detail (a int not null primary key, drop_me int references t_master(a));

";
}
