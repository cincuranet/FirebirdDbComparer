using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing
{
    public class RecursiveProcedure : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
				
";

        public override string Target => @"
set term ^;

create procedure new_procedure
as
begin
  execute procedure new_procedure;
end^

set term ;^
";
    }
}
