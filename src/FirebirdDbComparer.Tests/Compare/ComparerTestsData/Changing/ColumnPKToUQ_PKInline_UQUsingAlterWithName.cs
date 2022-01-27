using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class ColumnPKToUQ_PKInline_UQUsingAlterWithName : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create table t (id int not null);
alter table t add constraint uq_t unique (id);				
";

        public override string Target => @"
create table t (id int primary key);
";
    }
}