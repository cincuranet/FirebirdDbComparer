using System;
using FirebirdDbComparer.Exceptions;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class UDFSimpleModuleName : ComparerTests.TestCaseStructure
{
    public override string Source => @"
DECLARE EXTERNAL FUNCTION DLLVERSION
    BIGINT
RETURNS CSTRING(255) FREE_IT
ENTRY_POINT 'DLLVersion' MODULE_NAME 'Fast2';
";

    public override string Target => @"
DECLARE EXTERNAL FUNCTION DLLVERSION
    BIGINT
RETURNS CSTRING(255) FREE_IT
ENTRY_POINT 'DLLVersion' MODULE_NAME 'Fast';
";
}
