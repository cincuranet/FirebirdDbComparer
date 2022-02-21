using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class FunctionExternalEngine : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
create function new_ee_function(in1 integer)
returns integer
external name 'FirebirdDbComparer.Tests.FooBar!Foo.NewEEFunction'
engine FbNetExternalEngine;				
";

    public override string Target => @"

";
}
