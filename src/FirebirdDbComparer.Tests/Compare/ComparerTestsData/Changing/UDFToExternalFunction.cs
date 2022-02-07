using System;
using FirebirdDbComparer.Compare;
using FirebirdDbComparer.Exceptions;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class UDFToExternalFunction : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override Type ExpectedCompareException => typeof(CrossTypesOfSameObjectTypesException);

    public override string Source => @"
create function DLLVERSION(dummy BIGINT)
returns varchar(255)
external name 'FooBar!Foo.DllVersion'
engine FbNetExternalEngine;
";

    public override string Target => @"
DECLARE EXTERNAL FUNCTION DLLVERSION
    BIGINT
RETURNS CSTRING(255) FREE_IT
ENTRY_POINT 'DLLVersion' MODULE_NAME 'Fast';
";
}
