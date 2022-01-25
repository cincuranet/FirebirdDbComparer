using System;
using FirebirdDbComparer.Compare;
using FirebirdDbComparer.Exceptions;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class Trigger_DDL_FromBeforeToAfter : ComparerTests.TestCaseStructure
    {
        public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
        {
            return targetVersion.AtLeast(TargetVersion.Version30);
        }

        public override Type ExpectedCompareException => typeof(NotSupportedOnFirebirdException);

        public override string Source => @"
set term ^;

create trigger trig
before create package body
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
    -- foo
end^

set term ;^
";
    }
}
