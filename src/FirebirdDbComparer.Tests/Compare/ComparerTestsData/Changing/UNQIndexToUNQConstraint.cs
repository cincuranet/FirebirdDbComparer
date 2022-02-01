using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class UNQIndexToUNQConstraint : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (i int);
alter table t add constraint unq_i unique(i) using index unq_i;				
";

    public override string Target => @"
create table t (i int);
create unique index unq_i on t(i);
";
}
