using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class Trigger_A_DropFunctionDropExceptionCreateFunctionAlterRoleCreatePackageDropMapping_Active : ComparerTests.TestCaseStructure
    {
        public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
        {
            return targetVersion.AtLeast(TargetVersion.Version30);
        }

        public override string Source => @"
set term ^;

create trigger trig
active
after create package or drop function or create function or drop exception or alter role or drop mapping
as
begin
end^

set term ;^				
";

        public override string Target => @"

";
    }
}
