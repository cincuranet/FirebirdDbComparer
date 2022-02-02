using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class TableWith38NumericDecimal : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version40);
    }

    public override string Source => @"
create table t (d38 decimal(38, 19), n38 numeric(38, 19), d20 decimal(20, 4), n20 numeric(20, 4), d22o decimal(22), n22o numeric(22));
";

    public override string Target => @"

";
}
