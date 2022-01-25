using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class Index_ASC_MultipleFields_Unique : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create table t (i int, j date);

create unique asc index idx
on t(i, j);				
";

        public override string Target => @"

";
    }
}
