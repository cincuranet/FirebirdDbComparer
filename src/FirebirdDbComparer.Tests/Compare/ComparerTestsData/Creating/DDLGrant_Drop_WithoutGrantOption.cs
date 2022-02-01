using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class DDLGrant_Drop_WithoutGrantOption : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
grant drop any COLLATION TO PUBLIC;
grant drop any DOMAIN TO PUBLIC;
grant drop any EXCEPTION TO PUBLIC;
grant drop any FILTER TO PUBLIC;
grant drop any FUNCTION TO PUBLIC;
grant drop any SEQUENCE TO PUBLIC;
grant drop any PACKAGE TO PUBLIC;
grant drop any PROCEDURE TO PUBLIC;
grant drop any ROLE TO PUBLIC;
grant drop any TABLE TO PUBLIC;
grant drop any VIEW TO PUBLIC;
grant drop any CHARACTER SET TO PUBLIC;				
";

    public override string Target => @"

";
}
