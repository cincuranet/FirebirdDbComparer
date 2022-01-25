using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing
{
    public class ProceduresWithCycle : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
				
";

        public override string Target => @"
set term ^;

create procedure p1
as
begin
end^

create procedure p2
as
begin
    execute procedure p1;
end^

alter procedure p1
as
begin
    execute procedure p2;
end^

set term ;^
";
    }
}
