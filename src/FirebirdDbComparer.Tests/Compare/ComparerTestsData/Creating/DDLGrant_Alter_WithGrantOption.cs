using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class DDLGrant_Alter_WithGrantOption : ComparerTests.TestCaseStructure
    {
        public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
        {
            return targetVersion.AtLeast(TargetVersion.Version30);
        }

        public override string Source => @"
grant alter any COLLATION TO PUBLIC WITH GRANT OPTION;
grant alter any DOMAIN TO PUBLIC WITH GRANT OPTION;
grant alter any EXCEPTION TO PUBLIC WITH GRANT OPTION;
grant alter any FILTER TO PUBLIC WITH GRANT OPTION;
grant alter any FUNCTION TO PUBLIC WITH GRANT OPTION;
grant alter any SEQUENCE TO PUBLIC WITH GRANT OPTION;
grant alter any PACKAGE TO PUBLIC WITH GRANT OPTION;
grant alter any PROCEDURE TO PUBLIC WITH GRANT OPTION;
grant alter any ROLE TO PUBLIC WITH GRANT OPTION;
grant alter any TABLE TO PUBLIC WITH GRANT OPTION;
grant alter any VIEW TO PUBLIC WITH GRANT OPTION;
grant alter any CHARACTER SET TO PUBLIC WITH GRANT OPTION;				
";

        public override string Target => @"

";
    }
}