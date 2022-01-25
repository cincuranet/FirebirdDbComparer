using System;
using FirebirdDbComparer.Exceptions;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing
{
    public class ViewWithSameNameAsTableWithDependenciesInSource : ComparerTests.TestCaseStructure
    {
        public override Type ExpectedCompareException => typeof(CrossTypesOfSameObjectTypesException);

        public override string Source => @"
create table t (
    i  int
);

create table t_test (
    i  int,
    j  computed by ((select first 1 i from t))
);

create view v_test(i)
as
select * from t;
				
";

        public override string Target => @"
create view t(i)
as
select 1 as i from rdb$database;

create table t_test (
    i  integer,
    j  computed by ((select first 1 i from t))
);

create view v_test(i)
as
select * from t;

";
    }
}
