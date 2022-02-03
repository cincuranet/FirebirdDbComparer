using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class AddColumnWithIdentityAlwaysWithStartWithWithIncrementToTable : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version40);
    }

    public override string Source => @"
create table t (a int, b bigint generated always as identity (start with 99 increment by 6));				
";

    public override string Target => @"
create table t (a int);
";
}
