using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class ColumnPKToPK_PKUsingAlterWithName_PKInline : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create table t (id int primary key);				
";

        public override string Target => @"
create table t (id int not null);
alter table t add constraint pk_t primary key (id);
";
    }
}
