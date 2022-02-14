using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class ProcedureSQLSecurityFromInvokerToDefiner : ComparerTests.TestCaseStructure
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
set term ^;

create procedure p
sql security invoker
as
begin
end^

set term ;^
";
}

