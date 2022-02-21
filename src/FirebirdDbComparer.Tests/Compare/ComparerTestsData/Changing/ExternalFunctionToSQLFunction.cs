using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class ExternalFunctionToSQLFunction : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
set term ^;

CREATE FUNCTION DLLVERSION(dummy BIGINT) RETURNS VARCHAR(255)
AS
BEGIN
    RETURN 'Fast';
END^

set term ;^
";

    public override string Target => @"
create function DLLVERSION(dummy BIGINT)
returns varchar(255)
external name 'FirebirdDbComparer.Tests.FooBar!Foo.DllVersion'
engine FbNetExternalEngine;
";
}
