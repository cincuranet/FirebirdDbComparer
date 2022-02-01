using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class ColumnWithPKUsedInFK_AllUsingAlterWithName : ComparerTests.TestCaseStructure
{
    public override void AssertScript(ScriptResult compareResult)
    {
        var commands = compareResult.AllStatements.ToArray();
        Assert.That(commands.Count(), Is.EqualTo(3));
    }

    public override string Source => @"
create table t_master (i int);
create table t_detail (a int not null);
alter table t_detail add constraint pk_detail primary key (a);	
";

    public override string Target => @"
create table t_master (i int, a int not null);
alter table t_master add constraint pk_master primary key (a);
create table t_detail (a int not null, drop_me int);
alter table t_detail add constraint pk_detail primary key (a);
alter table t_detail add constraint fk_detail_master foreign key (drop_me) references t_master(a);

";
}
