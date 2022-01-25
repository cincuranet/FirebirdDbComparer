using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing
{
    public class ColumnUsedInCHK_Inline : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create table t (a int check (a < 66));				
";

        public override string Target => @"
create table t (a int, b int check (a < b));
";
    }
}
