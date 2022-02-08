namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class UDFComment : ComparerTests.TestCaseStructure
{
    public override string Source => @"
DECLARE EXTERNAL FUNCTION DLLVERSION
    INTEGER
RETURNS CSTRING(255) FREE_IT
ENTRY_POINT 'DLLVersion' MODULE_NAME 'Fast';

comment on external function DLLVERSION is 'test';				
";

    public override string Target => @"
DECLARE EXTERNAL FUNCTION DLLVERSION
    INTEGER
RETURNS CSTRING(255) FREE_IT
ENTRY_POINT 'DLLVersion' MODULE_NAME 'Fast';
";
}
