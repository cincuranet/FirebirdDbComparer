using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class DomainDefaultUsedInColumnWithDroppedIndex : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create domain d_test int;
create table t(col d_test);				
";

        public override string Target => @"
create domain d_test int default 10;
create table t(col d_test);
create index idx_t_col on t(col);
";
    }
}
