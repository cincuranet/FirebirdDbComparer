using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class PackageProcedureUsingNewTableNewColumnViaTypeOfColumn : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
create table test (c bigint);

set term ^;

create package some_pkg
as
begin
  procedure test(i type of column test.c);
end^

set term ;^				
";

    public override string Target => @"

";
}
