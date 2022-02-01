using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class ColumnDefault : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (a char(1), b char(1) DeFaUlT 'b');				
";

    public override string Target => @"
create table t (a char(1) dEfAuLt 'a', b char(1));
";
}
