using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class FunctionExternalEngine : ComparerTests.TestCaseStructure
    {
        public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
        {
            return targetVersion.AtLeast(TargetVersion.Version30);
        }

        public override string Source => @"
create function new_ee_function(in1 integer)
returns integer
external name 'FooBar!Foo.NewEEFunction'
engine FbNetExternalEngine;				
";

        public override string Target => @"

";
    }
}