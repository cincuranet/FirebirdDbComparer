using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class IndexAddColumn : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (a int, b int);
create index idx_t on t(a, b);				
";

    public override string Target => @"
create table t (a int, b int);
create index idx_t on t(a);
";
}
