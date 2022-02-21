using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class TriggerExternalEngine : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
create table t (id int);

create trigger new_ee_trigger
after update on t
external name 'FirebirdDbComparer.Tests.FooBar!Foo.NewEETrigger'
engine FbNetExternalEngine;				
";

    public override string Target => @"
create table t (id int);
";
}
