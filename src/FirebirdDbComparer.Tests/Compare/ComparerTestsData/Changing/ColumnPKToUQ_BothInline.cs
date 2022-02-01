using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class ColumnPKToUQ_BothInline : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (id int not null unique);				
";

    public override string Target => @"
create table t (id int primary key);
";
}
