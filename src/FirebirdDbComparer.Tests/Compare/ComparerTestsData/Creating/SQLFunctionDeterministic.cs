using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class SQLFunctionDeterministic : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
set term ^;

create or alter function new_function (i int) returns int deterministic
as
begin
end^

set term ;^				
";

    public override string Target => @"

";
}
