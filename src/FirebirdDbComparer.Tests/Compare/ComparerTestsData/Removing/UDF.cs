namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class UDF : ComparerTests.TestCaseStructure
{
    public override string Source => @"
				
";

    public override string Target => @"
DECLARE EXTERNAL FUNCTION DLLVERSION
    INTEGER
RETURNS CSTRING(255) FREE_IT
ENTRY_POINT 'DLLVersion' MODULE_NAME 'Fast';
";
}
