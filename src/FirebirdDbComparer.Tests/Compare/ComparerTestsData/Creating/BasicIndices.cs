using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class BasicIndices : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table t (i int);

create ascending index idx_t_i_asc on t(i);
create unique ascending index uq_idx_t_i_asc on t(i);
create descending index idx_t_i_desc on t(i);
create unique descending index uq_idx_t_i_desc on t(i);				
";

    public override string Target => @"

";
}
