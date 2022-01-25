using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class BasicSQLFunctionWithTypeOf : ComparerTests.TestCaseStructure
    {
        public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
        {
            return targetVersion.AtLeast(TargetVersion.Version30);
        }

        public override string Source => @"
create domain d_test as varchar(20);

set term ^;

create or alter function new_function (i type of d_test) returns type of d_test
as
begin
end^

set term ;^				
";

        public override string Target => @"
create domain d_test as varchar(20);
";
    }
}
