using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class RemoveSQLSecurityDefinerFromProcedure : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version40);
    }

    public override string Source => @"
set term ^;

create procedure p
as
begin
end^

set term ;^
";

    public override string Target => @"
set term ^;

create procedure p
sql security definer
as
begin
end^

set term ;^
";
}
