using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing
{
    public class DependingProcedure : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
				
";

        public override string Target => @"
create domain d_test as int;

set term ^;

create or alter procedure new_procedure (
    in1 varchar(10) character set utf8 collate ucs_basic,
    in2 d_test,
    in3 type of d_test)
returns (
    out1 integer,
    out2 d_test,
    out3 type of d_test)
as
begin
  exit;
end^

create or alter procedure new_procedure_2 (
    in1 varchar(10) character set utf8 collate ucs_basic,
    in2 d_test,
    in3 type of d_test)
returns (
    out1 integer,
    out2 d_test,
    out3 type of d_test)
as
declare v_in1 varchar(10) character set utf8 collate ucs_basic;
declare v_in2 d_test;
declare v_in3 type of d_test;
begin
  execute procedure new_procedure(in1, in2, in3)
  returning_values
    v_in1,
    v_in2,
    v_in3;
end^

set term ;^
";
    }
}
