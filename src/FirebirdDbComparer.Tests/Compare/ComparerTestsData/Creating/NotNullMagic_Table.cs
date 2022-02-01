using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class NotNullMagic_Table : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create domain d_int as int;
create domain d_int_nn as int not null;

create table t (
    a int,
    b int not null,
    c d_int,
    d d_int not null,
    e d_int_nn,
    f d_int_nn not null);				
";

    public override string Target => @"

";
}
