using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class ColumnCharVarcharLengthInTable : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (a char(2), b varchar(2));				
";

    public override string Target => @"
create table t (a char(1), b varchar(1));
";
}
