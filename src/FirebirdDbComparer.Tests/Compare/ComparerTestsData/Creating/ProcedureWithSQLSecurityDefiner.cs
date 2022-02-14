using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class ProcedureWithSQLSecurityDefiner : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version40);
    }

    public override string Source => @"
set term ^;

create procedure p
sql security definer
as
begin
end^

set term ;^
";

    public override string Target => @"

";
}
