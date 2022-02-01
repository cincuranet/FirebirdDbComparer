using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class SQLFunction : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
				
";

    public override string Target => @"
set term ^;

create function new_function(i int) returns int
as
begin
 return i + 1;
end^

set term ;^
";
}
