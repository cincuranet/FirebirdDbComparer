using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class AddFKChangePKDataTypeToDomain : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create domain d_primary bigint not null;
create domain d_foreign_nn bigint not null;
create table parent (id d_primary not null);
create table child (id d_primary not null, id_parent d_foreign_nn not null);
alter table parent add constraint pk_parent primary key (id);
alter table child add constraint pk_child primary key (id);
alter table child add constraint fk_child_parent foreign key (id_parent) references parent(id);				
";

        public override string Target => @"
create domain d_primary bigint not null;
create domain d_foreign_nn bigint not null;
create table parent (id bigint not null);
create table child (id bigint not null, id_parent bigint not null);
alter table parent add constraint pk_parent primary key (id);
alter table child add constraint pk_child primary key (id);
";
    }
}
