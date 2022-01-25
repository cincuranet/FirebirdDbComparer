using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class ColumnPKToUQ_PKUsingAlterWithName_UQInline : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create table t (id int not null);
alter table t add constraint pk_t primary key (id);				
";

        public override string Target => @"
create table t (id int not null unique);
";
    }
}
