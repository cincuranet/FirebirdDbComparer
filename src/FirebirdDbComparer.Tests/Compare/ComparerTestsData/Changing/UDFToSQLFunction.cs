using System;
using FirebirdDbComparer.Compare;
using FirebirdDbComparer.Exceptions;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class UDFToSQLFunction : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override Type ExpectedCompareException => typeof(CrossTypesOfSameObjectTypesException);

    public override string Source => @"
set term ^;

CREATE FUNCTION DLLVERSION(I INT) RETURNS VARCHAR(255)
AS
BEGIN
    RETURN 'Fast';
END^

set term ;^
";

    public override string Target => @"
DECLARE EXTERNAL FUNCTION DLLVERSION
    BIGINT
RETURNS CSTRING(255) FREE_IT
ENTRY_POINT 'DLLVersion' MODULE_NAME 'Fast';
";
}
