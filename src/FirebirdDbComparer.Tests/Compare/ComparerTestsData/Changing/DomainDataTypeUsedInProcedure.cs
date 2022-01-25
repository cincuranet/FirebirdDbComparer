using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class DomainDataTypeUsedInProcedure : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create domain number as int;

set term ^;

create procedure p (in_param number)
returns (out_param number)
as
declare var_param number;
begin
end^

set term ;^				
";

        public override string Target => @"
create domain number as smallint;

set term ^;

create procedure p (in_param number)
returns (out_param number)
as
declare var_param number;
begin
end^

set term ;^
";
    }
}
