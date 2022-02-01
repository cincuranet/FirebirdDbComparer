using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class TableWithComputedFields : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table aaa (i int);
create table zzz (i int);

create table table_cf1 (
    i int);
create table table_cf2 (
    i int,
    j computed by (i + i),
    k computed by (j + i),
    l varchar(20),
    m computed by (trim(l)));

alter table table_cf2 add a computed by (j + (select first 1 i from zzz));
alter table table_cf2 add b computed by (j + (select first 1 i from aaa));
alter table table_cf2 add c computed by (j + (select first 1 i from table_cf2));

alter table table_cf1 add a computed by ((select first 1 a from table_cf2));				
";

    public override string Target => @"

";
}
