using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class DomainCheck : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create domain d_change as int check (value > 66);
create domain d_create as int check (value > 0);
create domain d_drop as int;				
";

    public override string Target => @"
create domain d_change as int check (value > 6);
create domain d_create as int;
create domain d_drop as int check (value > 0);
";
}
