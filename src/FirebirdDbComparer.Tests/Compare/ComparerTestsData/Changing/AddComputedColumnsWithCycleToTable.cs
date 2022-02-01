using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class AddComputedColumnsWithCycleToTable : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (a int);
alter table t add b computed by (1);
alter table t add c computed by (b+b);
alter table t alter b computed by (a+a);				
";

    public override string Target => @"
create table t (a int);
";
}
