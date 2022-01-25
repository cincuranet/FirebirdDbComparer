using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class Index_DESC_OneField_Inactive : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create table t (i int);

create desc index idx
on t(i);
alter index idx inactive;				
";

        public override string Target => @"

";
    }
}
