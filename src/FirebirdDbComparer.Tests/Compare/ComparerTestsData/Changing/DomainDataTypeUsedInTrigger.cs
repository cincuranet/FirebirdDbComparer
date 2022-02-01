using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class DomainDataTypeUsedInTrigger : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create domain number as int;

set term ^;

create trigger trg
active on connect
as
declare var number;
begin
end^

set term ;^				
";

    public override string Target => @"
create domain number as smallint;

set term ^;

create trigger trg
active on connect
as
declare var number;
begin
end^

set term ;^
";
}
