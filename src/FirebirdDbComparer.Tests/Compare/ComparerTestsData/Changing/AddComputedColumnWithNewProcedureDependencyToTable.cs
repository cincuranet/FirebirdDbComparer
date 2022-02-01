using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class AddComputedColumnWithNewProcedureDependencyToTable : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (a int);

set term ^;

create procedure p (in_i int)
returns (out_i int)
as
begin
    out_i = in_i;
    suspend;
end^

set term ;^

alter table t add b computed by ((select out_i from p(a)));				
";

    public override string Target => @"
create table t (a int);
";
}
