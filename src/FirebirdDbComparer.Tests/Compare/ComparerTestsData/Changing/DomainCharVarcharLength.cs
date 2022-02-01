using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class DomainCharVarcharLength : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create domain d1 as char(2);
create domain d2 as varchar(2);

create table t (a d1, b d2);				
";

    public override string Target => @"
create domain d1 as char(1);
create domain d2 as varchar(1);

create table t (a d1, b d2);
";
}
