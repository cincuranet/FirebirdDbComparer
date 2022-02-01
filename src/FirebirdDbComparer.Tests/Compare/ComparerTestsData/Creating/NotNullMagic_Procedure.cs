using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class NotNullMagic_Procedure : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create domain d_int as int;
create domain d_int_nn as int not null;

set term ^;

create procedure p (
    in_a int,
    in_b int not null,
    in_c d_int,
    in_d d_int not null,
    in_e d_int_nn,
    in_f d_int_nn not null,
    in_g type of d_int,
    in_h type of d_int not null,
    in_i type of d_int_nn,
    in_j type of d_int_nn not null,
    in_k type of column rdb$database.rdb$description,
    in_l type of column rdb$database.rdb$description not null)
returns (
    out_a int,
    out_b int not null,
    out_c d_int,
    out_d d_int not null,
    out_e d_int_nn,
    out_f d_int_nn not null,
    out_g type of d_int,
    out_h type of d_int not null,
    out_i type of d_int_nn,
    out_j type of d_int_nn not null,
    out_k type of column rdb$database.rdb$description,
    out_l type of column rdb$database.rdb$description not null)
as
begin
end^

set term ;^				
";

    public override string Target => @"

";
}
