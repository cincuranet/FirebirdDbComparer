using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class AddColumnToTableWithPosition : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (x varchar(20), a int);				
";

    public override string Target => @"
create table t (a int);
";
}
