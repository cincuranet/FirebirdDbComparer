using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class SQLFunctionToExternalFunction : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
create function DLLVERSION(dummy BIGINT)
returns varchar(255)
external name 'FooBar!Foo.DllVersion'
engine FbNetExternalEngine;
";

    public override string Target => @"
set term ^;

CREATE FUNCTION DLLVERSION(dummy BIGINT) RETURNS VARCHAR(255)
AS
BEGIN
    RETURN 'Fast';
END^

set term ;^
";
}
