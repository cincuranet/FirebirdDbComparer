using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class FunctionExternalEngineUsedByExistingTrigger : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
create table t (i int);

create function new_ee_function(in1 integer)
returns integer
external name 'FirebirdDbComparer.Tests.FooBar!Foo.NewEEFunction'
engine FbNetExternalEngine;

set term ~;
create trigger trig for t
before insert
as
begin
    new.i = new_ee_function(new.i);
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
