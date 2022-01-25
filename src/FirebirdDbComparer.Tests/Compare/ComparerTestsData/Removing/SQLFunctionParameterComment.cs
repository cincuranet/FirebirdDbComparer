using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing
{
    public class SQLFunctionParameterComment : ComparerTests.TestCaseStructure
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
    return i;
end^

set term ;^				
";

        public override string Target => @"
set term ^;

create or alter function new_function (i int) returns int
as
begin
    return i;
end^

set term ;^

comment on parameter new_function.i is 'test';
";
    }
}
