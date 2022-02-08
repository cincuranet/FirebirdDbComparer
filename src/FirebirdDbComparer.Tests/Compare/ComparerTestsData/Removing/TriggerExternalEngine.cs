using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class TriggerExternalEngine : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version30);
    }

    public override string Source => @"
create table t (id int);				
";

    public override string Target => @"
create table t (id int);

create trigger new_ee_trigger
after update on t
external name 'FooBar!Foo.NewEETrigger'
engine FbNetExternalEngine;
";
}
