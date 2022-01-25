using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class ColumnRawDataTypeInTable : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create table t (a bigint);				
";

        public override string Target => @"
create table t (a smallint);
";
    }
}
