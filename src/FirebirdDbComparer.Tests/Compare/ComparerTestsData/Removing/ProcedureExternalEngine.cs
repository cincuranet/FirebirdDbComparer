using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class ProcedureExternalEngine : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
				
";

    public override string Target => @"
create procedure new_ee_procedure(in1 integer)
returns (out1 integer)
external name 'FirebirdDbComparer.Tests.FooBar!Foo.NewEEProcedure'
engine FbNetExternalEngine;
";
}
