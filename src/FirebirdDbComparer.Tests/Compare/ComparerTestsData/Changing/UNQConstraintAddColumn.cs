using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class UNQConstraintAddColumn : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (a int, b int);
alter table t add constraint unq_t unique (a, b) using index unq_t;				
";

    public override string Target => @"
create table t (a int, b int);
alter table t add constraint unq_t unique (a) using index unq_t;
";
}
