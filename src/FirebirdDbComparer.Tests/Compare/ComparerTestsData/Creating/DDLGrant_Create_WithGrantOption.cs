using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class DDLGrant_Create_WithGrantOption : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
grant create COLLATION TO PUBLIC WITH GRANT OPTION;
grant create DOMAIN TO PUBLIC WITH GRANT OPTION;
grant create EXCEPTION TO PUBLIC WITH GRANT OPTION;
grant create FILTER TO PUBLIC WITH GRANT OPTION;
grant create FUNCTION TO PUBLIC WITH GRANT OPTION;
grant create SEQUENCE TO PUBLIC WITH GRANT OPTION;
grant create PACKAGE TO PUBLIC WITH GRANT OPTION;
grant create PROCEDURE TO PUBLIC WITH GRANT OPTION;
grant create ROLE TO PUBLIC WITH GRANT OPTION;
grant create TABLE TO PUBLIC WITH GRANT OPTION;
grant create VIEW TO PUBLIC WITH GRANT OPTION;
grant create CHARACTER SET TO PUBLIC WITH GRANT OPTION;				
";

    public override string Target => @"

";
}
