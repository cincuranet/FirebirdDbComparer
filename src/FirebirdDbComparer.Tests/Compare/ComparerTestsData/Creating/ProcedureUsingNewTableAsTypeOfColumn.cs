using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class ProcedureUsingNewTableAsTypeOfColumn : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (i int);

set term ^;

create procedure p (i type of column t.i)
as
begin
end^

set term ;^				
";

    public override string Target => @"

";
}
