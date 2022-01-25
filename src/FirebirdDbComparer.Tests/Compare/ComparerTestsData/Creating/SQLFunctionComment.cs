using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class SQLFunctionComment : ComparerTests.TestCaseStructure
    {
        public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
        {
            return targetVersion.AtLeast(TargetVersion.Version30);
        }

        public override string Source => @"
set term ^;

create or alter function new_function (i int) returns int
as
begin
end^

set term ;^

comment on function new_function is 'test';				
";

        public override string Target => @"
set term ^;

create or alter function new_function (i int) returns int
as
begin
end^

set term ;^
";
    }
}
