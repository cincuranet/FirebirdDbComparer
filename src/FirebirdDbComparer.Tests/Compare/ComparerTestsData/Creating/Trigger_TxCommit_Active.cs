using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class Trigger_TxCommit_Active : ComparerTests.TestCaseStructure
{
    public override string Source => @"
set term ^;

create trigger trig
active
on transaction commit
as
begin
end^

set term ;^				
";

    public override string Target => @"

";
}
