using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class CHK_Inline : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create table t (
    col_change int check (col_change > 66),
    col_drop int,
    col_create int check (col_create > 0));				
";

        public override string Target => @"
create table t (
    col_change int check (col_change > 6),
    col_drop int check (col_drop > 0),
    col_create int);
";
    }
}
