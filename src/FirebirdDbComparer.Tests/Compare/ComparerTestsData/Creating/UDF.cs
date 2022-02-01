using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class UDF : ComparerTests.TestCaseStructure
{
    public override string Source => @"
DECLARE EXTERNAL FUNCTION DLLVERSION
    INTEGER
RETURNS CSTRING(255) FREE_IT
ENTRY_POINT 'DLLVersion' MODULE_NAME 'Fast';				
";

    public override string Target => @"

";
}
