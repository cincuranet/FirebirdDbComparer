using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing
{
    public class TableWithAllRelationConstraint : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
				
";

        public override string Target => @"
create table my_table (
  a int primary key,
  b int not null,
  c int check(a is not null),
  d int not null unique
);

create table another_table (
  a int primary key references my_table(d),
  b int references my_table(a)
);
";
    }
}
