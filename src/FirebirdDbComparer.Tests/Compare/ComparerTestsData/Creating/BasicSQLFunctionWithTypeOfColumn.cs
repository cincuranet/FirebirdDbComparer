using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class BasicSQLFunctionWithTypeOfColumn : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
create table t (i int);

set term ^;

create or alter function new_function (i type of column t.i) returns type of column t.i
as
begin
end^

set term ;^				
";

    public override string Target => @"
create table t (i int);
";
}
