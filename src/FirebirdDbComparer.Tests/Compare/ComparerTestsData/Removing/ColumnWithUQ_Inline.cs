using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing
{
    public class ColumnWithUQ_Inline : ComparerTests.TestCaseStructure
    {
        public override void AssertScript(ScriptResult compareResult)
        {
            var commands = compareResult.AllStatements.ToArray();
            Assert.That(commands.Count(), Is.EqualTo(1));
        }

        public override string Source => @"
create table t (b char(20));				
";

        public override string Target => @"
create table t (a varchar(20) unique, b char(20));
";
    }
}
