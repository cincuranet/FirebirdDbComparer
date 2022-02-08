using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class DDLGrant_Alter_WithoutGrantOption : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
grant alter any COLLATION TO PUBLIC;
grant alter any DOMAIN TO PUBLIC;
grant alter any EXCEPTION TO PUBLIC;
grant alter any FILTER TO PUBLIC;
grant alter any FUNCTION TO PUBLIC;
grant alter any SEQUENCE TO PUBLIC;
grant alter any PACKAGE TO PUBLIC;
grant alter any PROCEDURE TO PUBLIC;
grant alter any ROLE TO PUBLIC;
grant alter any TABLE TO PUBLIC;
grant alter any VIEW TO PUBLIC;
grant alter any CHARACTER SET TO PUBLIC;				
";

    public override string Target => @"

";
}
