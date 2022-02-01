using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class DDLGrant_Drop_WithGrantOption : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
grant drop any COLLATION TO PUBLIC WITH GRANT OPTION;
grant drop any DOMAIN TO PUBLIC WITH GRANT OPTION;
grant drop any EXCEPTION TO PUBLIC WITH GRANT OPTION;
grant drop any FILTER TO PUBLIC WITH GRANT OPTION;
grant drop any FUNCTION TO PUBLIC WITH GRANT OPTION;
grant drop any SEQUENCE TO PUBLIC WITH GRANT OPTION;
grant drop any PACKAGE TO PUBLIC WITH GRANT OPTION;
grant drop any PROCEDURE TO PUBLIC WITH GRANT OPTION;
grant drop any ROLE TO PUBLIC WITH GRANT OPTION;
grant drop any TABLE TO PUBLIC WITH GRANT OPTION;
grant drop any VIEW TO PUBLIC WITH GRANT OPTION;
grant drop any CHARACTER SET TO PUBLIC WITH GRANT OPTION;				
";

    public override string Target => @"

";
}
