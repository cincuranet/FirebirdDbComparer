using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class DomainUsedInPKThatsUsedInFK : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create domain number as int;
-- the names are 'a' and 'b' try to confuse the ordering
create table b (i number not null);
alter table b add constraint pk_b primary key (i);
create table a (i number not null);
alter table a add constraint pk_a primary key (i);
alter table a add constraint fk_a foreign key (i) references b(i);				
";

        public override string Target => @"
create domain number as smallint;
-- the names are 'a' and 'b' try to confuse the ordering
create table b (i number not null);
alter table b add constraint pk_b primary key (i);
create table a (i number not null);
alter table a add constraint pk_a primary key (i);
alter table a add constraint fk_a foreign key (i) references b(i);
";
    }
}
