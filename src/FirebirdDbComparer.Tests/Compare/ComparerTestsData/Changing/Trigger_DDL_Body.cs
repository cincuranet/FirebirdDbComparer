using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class Trigger_DDL_Body : ComparerTests.TestCaseStructure
    {
        public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
        {
            return targetVersion.AtLeast(TargetVersion.Version30);
        }

        public override string Source => @"
set term ^;

create trigger trig
after create package body
as
begin
    -- foo
end^

set term ;^				
";

        public override string Target => @"
set term ^;

create trigger trig
after create package body
as
begin
    -- foobar
end^

set term ;^
";
    }
}
