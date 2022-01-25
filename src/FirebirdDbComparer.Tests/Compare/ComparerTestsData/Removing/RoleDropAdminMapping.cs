using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing
{
    public class RoleDropAdminMapping : ComparerTests.TestCaseStructure
    {
        public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
        {
            return targetVersion.AtMost(TargetVersion.Version25);
        }

        public override string Source => @"
alter role rdb$admin drop auto admin mapping;				
";

        public override string Target => @"
alter role rdb$admin set auto admin mapping;
";
    }
}
