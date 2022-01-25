using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class ProcedureComment : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
set term ^;

create or alter procedure new_procedure
as
begin
end^

set term ;^

comment on procedure new_procedure is 'test';				
";

        public override string Target => @"
set term ^;

create or alter procedure new_procedure
as
begin
end^

set term ;^
";
    }
}
