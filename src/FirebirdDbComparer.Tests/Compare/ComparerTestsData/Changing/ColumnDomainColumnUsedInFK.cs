using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class ColumnDomainColumnUsedInFK : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create domain fk as int;
create domain fk_nn as int not null;
create table master (id int primary key);
create table detail (id int primary key, id_master fk);
alter table detail add foreign key (id_master) references master(id);				
";

        public override string Target => @"
create domain fk as int;
create domain fk_nn as int not null;
create table master (id int primary key);
create table detail (id int primary key, id_master fk_nn);
alter table detail add foreign key (id_master) references master(id);
";
    }
}
