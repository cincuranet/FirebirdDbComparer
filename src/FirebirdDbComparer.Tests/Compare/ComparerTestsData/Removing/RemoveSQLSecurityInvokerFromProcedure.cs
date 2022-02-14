using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class RemoveSQLSecurityInvokerFromProcedure : ComparerTests.TestCaseStructure
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
sql security invoker
as
begin
end^

set term ;^
";
}
