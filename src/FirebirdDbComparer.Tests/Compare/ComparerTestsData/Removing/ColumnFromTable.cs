using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing
{
    public class ColumnFromTable : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create table t (b char(20));				
";

        public override string Target => @"
create table t (a varchar(20), b char(20));
";
    }
}
