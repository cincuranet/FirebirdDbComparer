using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class EmptyDatabase : ComparerTests.TestCaseStructure
    {
        public override void AssertScript(ScriptResult compareResult)
        {
            Assert.That(compareResult.AllStatements, Is.Empty);
        }

        public override string Source => @"
				
";

        public override string Target => @"

";
    }
}
