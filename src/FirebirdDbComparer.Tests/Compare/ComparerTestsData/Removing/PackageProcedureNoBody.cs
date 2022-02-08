using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class PackageProcedureNoBody : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
				
";

    public override string Target => @"
set term ^;

create package some_pkg
as
begin
  procedure test(i int);
end^

set term ;^
";
}
