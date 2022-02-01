using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

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
external name 'FooBar!Foo.NewEEProcedure'
engine FbNetExternalEngine;
";
}
