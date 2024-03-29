using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class DDLGrant_Create_WithoutGrantOption : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
				
";

    public override string Target => @"
grant create COLLATION TO PUBLIC;
grant create DOMAIN TO PUBLIC;
grant create EXCEPTION TO PUBLIC;
grant create FILTER TO PUBLIC;
grant create FUNCTION TO PUBLIC;
grant create SEQUENCE TO PUBLIC;
grant create PACKAGE TO PUBLIC;
grant create PROCEDURE TO PUBLIC;
grant create ROLE TO PUBLIC;
grant create TABLE TO PUBLIC;
grant create VIEW TO PUBLIC;
grant create CHARACTER SET TO PUBLIC;
";
}
