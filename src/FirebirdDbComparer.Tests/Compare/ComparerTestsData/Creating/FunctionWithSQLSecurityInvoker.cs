using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class FunctionWithSQLSecurityInvoker : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version40);
    }

    public override string Source => @"
set term ^;

create function f
returns int
sql security invoker
as
begin
    return 6;
end^

set term ;^
";

    public override string Target => @"

";
}
