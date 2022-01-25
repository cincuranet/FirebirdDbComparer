using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class ConstraintUQ_UsingAlterWithName : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create table t (i int);
alter table t add constraint uq_t unique (i);				
";

        public override string Target => @"

";
    }
}
