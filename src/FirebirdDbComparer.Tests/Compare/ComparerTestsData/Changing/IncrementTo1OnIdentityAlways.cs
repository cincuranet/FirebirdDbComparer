using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class IncrementTo1OnIdentityAlways : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version40);
    }

    public override string Source => @"
create table t(i int generated always as identity (increment by 1))				
";

    public override string Target => @"
create table t(i int generated always as identity (increment by 2))	
";
}
