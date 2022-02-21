using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class ProcedureExternalEngineUsedByExistingTrigger : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
create table t (i int);

create procedure new_ee_procedure(in1 integer)
returns (out1 integer)
external name 'FirebirdDbComparer.Tests.FooBar!Foo.NewEEProcedure'
engine FbNetExternalEngine;

set term ~;
create trigger trig for t
before insert
as
begin
    new.i = (select out1 from new_ee_procedure(new.i));
end~
set term ;~				
";

    public override string Target => @"
create table t (i int);

set term ~;
create trigger trig for t
before insert
as
begin
end~
set term ;~
";
}
