using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class Index_DESC_MultipleFields : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create table t (i int, j date);

create desc index idx
on t(i, j);				
";

        public override string Target => @"

";
    }
}
