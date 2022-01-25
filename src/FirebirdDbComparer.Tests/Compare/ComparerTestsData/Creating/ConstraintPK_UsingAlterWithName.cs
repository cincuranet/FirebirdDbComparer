using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class ConstraintPK_UsingAlterWithName : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create table t (i int not null);
alter table t add constraint pk_t2 primary key (i);				
";

        public override string Target => @"

";
    }
}
