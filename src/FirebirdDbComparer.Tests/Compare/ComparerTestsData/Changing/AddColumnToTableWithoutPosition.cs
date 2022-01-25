using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class AddColumnToTableWithoutPosition : ComparerTests.TestCaseStructure
    {
        public override void AssertScript(ScriptResult compareResult)
        {
            var commands = compareResult.AllStatements.ToArray();
            Assert.That(commands.Count(), Is.EqualTo(1));
        }

        public override string Source => @"
create table t (a int, b varchar(20));
";

        public override string Target => @"
create table t (a int);
";
    }
}
