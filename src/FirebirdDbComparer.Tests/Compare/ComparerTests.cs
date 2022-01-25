using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FirebirdDbComparer.Compare;
using FirebirdDbComparer.Interfaces;

using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare
{
    public abstract class ComparerTests
    {
        public abstract class TestCaseStructure
        {
            private sealed class Dummy : TestCaseStructure
            {
                public override string Source => string.Empty;
                public override string Target => string.Empty;
            }

            public static readonly TestCaseStructure Empty = new Dummy();

            public virtual bool IsCompatibleWithVersion(TargetVersion targetVersion) => true;
            public virtual Type ExpectedCompareException => null;
            public virtual void AssertScript(ScriptResult compareResult) { }

            public abstract string Source { get; }
            public abstract string Target { get; }
        }

        private readonly TargetVersion m_Version;

        public ComparerTests(TargetVersion version)
        {
            m_Version = version;
        }

        [SetUp]
        public void Setup()
        {
            Helpers.Database.CreateDatabase(m_Version, Helpers.Database.DatabaseLocation.Source);
            Helpers.Database.CreateDatabase(m_Version, Helpers.Database.DatabaseLocation.Target);
        }

        [TearDown]
        public void TearDown()
        {
            Helpers.Database.DropDatabase(m_Version, Helpers.Database.DatabaseLocation.Target);
            Helpers.Database.DropDatabase(m_Version, Helpers.Database.DatabaseLocation.Source);
        }

        private static void ExecuteSpecificAsserts(TestCaseStructure testCaseStructure, ScriptResult compareResult)
        {
            testCaseStructure.AssertScript(compareResult);
        }

        private Comparer CreateComparer(IComparerSettings settings)
        {
            return Comparer.ForTwoDatabases(
                settings,
                Helpers.Database.GetConnectionString(m_Version, Helpers.Database.DatabaseLocation.Source),
                Helpers.Database.GetConnectionString(m_Version, Helpers.Database.DatabaseLocation.Target));
        }

        private void AssertNoDifferenceBetweenSourceAndTarget()
        {
            var sourceStructure = default(string);
            var targetStructure = default(string);
            Parallel.Invoke(
                () => { sourceStructure = Helpers.Database.GetDatabaseStructure(m_Version, Helpers.Database.DatabaseLocation.Source); },
                () => { targetStructure = Helpers.Database.GetDatabaseStructure(m_Version, Helpers.Database.DatabaseLocation.Target); });
            try
            {
                Assert.That(targetStructure, Is.EqualTo(sourceStructure));
            }
            catch
            {
                TestContext.WriteLine("*** Source:");
                TestContext.WriteLine(sourceStructure);
                TestContext.WriteLine();
                TestContext.WriteLine("*** Target:");
                TestContext.WriteLine(targetStructure);
                TestContext.WriteLine();
                throw;
            }
        }

        private void AssertComparerSeesNoDifference(IComparerSettings settings)
        {
            var comparer = CreateComparer(settings);
            var commands = comparer.Compare().Statements;
            try
            {
                Assert.That(commands, Is.Empty);
            }
            catch
            {
                TestContext.WriteLine("*** Differences:");
                foreach (var item in commands)
                {
                    TestContext.WriteLine(item);
                }
                TestContext.WriteLine();
                throw;
            }
        }

        private ScriptResult UpdateDatabase(IComparerSettings settings, TestCaseStructure testCaseStructure)
        {
            var comparer = CreateComparer(settings);
            var compareResult = default(ScriptResult);
            try
            {
                compareResult = comparer.Compare().Script;
            }
            catch (Exception ex) when (testCaseStructure.ExpectedCompareException?.IsAssignableFrom(ex.GetType()) ?? false)
            {
                Assert.Pass();
                return null;
            }
            if (testCaseStructure.ExpectedCompareException != null)
            {
                Assert.Fail("Expected exception but nothing happened.");
                return null;
            }
            TestContext.WriteLine("*** Change script:");
            var commands = compareResult.AllStatements.ToArray();
            foreach (var item in commands)
            {
                TestContext.WriteLine(item);
            }
            TestContext.WriteLine();
            Helpers.Database.ExecuteScript(m_Version, Helpers.Database.DatabaseLocation.Target, commands);
            return compareResult;
        }

        private sealed class CompareSource : IEnumerable<TestCaseData>
        {
            public IEnumerator<TestCaseData> GetEnumerator()
            {
                var data = typeof(ComparerTests).Assembly.GetTypes()
                    .Where(x => !x.IsAbstract && x.IsPublic && x.IsAssignableTo(typeof(TestCaseStructure)))
                    .Select(x => x.FullName)
                    .Select(x =>
                    {
                        var result = new TestCaseData(x);
                        result.SetName(x);
                        return result;
                    });
                foreach (var item in data)
                    yield return item;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        }
        [TestCaseSource(typeof(CompareSource))]
        public void Compare(string name)
        {
            var testCaseStructure = (TestCaseStructure)Activator.CreateInstance(Type.GetType(name));
            if (!testCaseStructure.IsCompatibleWithVersion(m_Version))
            {
                Assert.Inconclusive($"Test is not compatible with {m_Version}.");
                return;
            }
            Parallel.Invoke(
                () => Helpers.Database.ExecuteScript(m_Version, Helpers.Database.DatabaseLocation.Source, testCaseStructure.Source),
                () => Helpers.Database.ExecuteScript(m_Version, Helpers.Database.DatabaseLocation.Target, testCaseStructure.Target));
            var settings = new ComparerSettings(m_Version);
            var compareResult = UpdateDatabase(settings, testCaseStructure);
            if (compareResult != null)
            {
                AssertNoDifferenceBetweenSourceAndTarget();
                AssertComparerSeesNoDifference(settings);
                ExecuteSpecificAsserts(testCaseStructure, compareResult);
            }
        }

        [Test]
        public void Settings_IgnorePermissions_True()
        {
            Parallel.Invoke(
                () => Helpers.Database.ExecuteScript(m_Version, Helpers.Database.DatabaseLocation.Source, new[] { "create table test(c int);", "grant all on test to someuser;" }),
                () => Helpers.Database.ExecuteScript(m_Version, Helpers.Database.DatabaseLocation.Target, new[] { "create table test(c int);" }));
            var settings = new ComparerSettings(m_Version) { IgnorePermissions = true };
            var compareResult = UpdateDatabase(settings, TestCaseStructure.Empty);
            Assert.That(compareResult.AllStatements, Is.Empty);
            AssertComparerSeesNoDifference(settings);
        }

        [Test]
        public void Settings_IgnorePermissions_False()
        {
            Parallel.Invoke(
                () => Helpers.Database.ExecuteScript(m_Version, Helpers.Database.DatabaseLocation.Source, new[] { "create table test(c int);", "grant all on test to someuser;" }),
                () => Helpers.Database.ExecuteScript(m_Version, Helpers.Database.DatabaseLocation.Target, new[] { "create table test(c int);" }));
            var settings = new ComparerSettings(m_Version) { IgnorePermissions = false };
            var compareResult = UpdateDatabase(settings, TestCaseStructure.Empty);
            Assert.That(compareResult.AllStatements.Count(), Is.GreaterThanOrEqualTo(1));
            Assert.That(compareResult.AllStatements.First(), Does.StartWith("GRANT "));
            AssertComparerSeesNoDifference(settings);
        }
    }

    [TestFixture]
    public sealed class ComparerTests25 : ComparerTests
    {
        public ComparerTests25()
            : base(TargetVersion.Version25)
        { }
    }

    [TestFixture]
    public sealed class ComparerTests30 : ComparerTests
    {
        public ComparerTests30()
            : base(TargetVersion.Version30)
        { }
    }

    [TestFixture]
    public sealed class ComparerTests40 : ComparerTests
    {
        public ComparerTests40()
            : base(TargetVersion.Version40)
        { }
    }
}
