using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class DomainDataTypeUsedInTable : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create domain number as int;
create table t (i number);				
";

    public override string Target => @"
create domain number as smallint;
create table t (i number);
";
}
