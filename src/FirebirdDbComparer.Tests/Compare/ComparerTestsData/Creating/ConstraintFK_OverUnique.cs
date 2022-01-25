using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class ConstraintFK_OverUnique : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create table t_master (i int);
alter table t_master add unique (i);
create table t_detail (i int primary key);
alter table t_detail add constraint fk_detail_master foreign key (i) references t_master(i);				
";

        public override string Target => @"

";
    }
}
