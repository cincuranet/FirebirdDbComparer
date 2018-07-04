using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using FirebirdDbComparer.Compare;
using FirebirdDbComparer.Interfaces;

using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare
{
    public abstract class ComparerTests
    {
        private readonly TargetVersion m_Version;

        public ComparerTests(TargetVersion version)
        {
            m_Version = version;
        }

        [SetUp]
        public void Setup()
        {
            Parallel.Invoke(
                () => Helpers.Database.CreateDatabase(m_Version, Helpers.Database.DatabaseLocation.Source),
                () => Helpers.Database.CreateDatabase(m_Version, Helpers.Database.DatabaseLocation.Target));
        }

        [TearDown]
        public void TearDown()
        {
            Parallel.Invoke(
                () => Helpers.Database.DropDatabase(m_Version, Helpers.Database.DatabaseLocation.Target),
                () => Helpers.Database.DropDatabase(m_Version, Helpers.Database.DatabaseLocation.Source));
        }

        public interface ITestCaseScriptSpecificAsserts
        {
            void Execute(ScriptResult compareResult);
        }
        private static void ExecuteSpecificAsserts(string dataName, ScriptResult compareResult)
        {
            var typeName = $"{typeof(ComparerTests).Assembly.GetName().Name}.{dataName}";
            var type = Type.GetType(typeName);
            if (type != null)
            {
                ((ITestCaseScriptSpecificAsserts)Activator.CreateInstance(type)).Execute(compareResult);
                TestContext.WriteLine("Specific asserts executed.");
            }
            else
            {
                TestContext.WriteLine("No specific asserts found.");
            }
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
                TestContext.WriteLine("Source:");
                TestContext.WriteLine(sourceStructure);
                TestContext.WriteLine();
                TestContext.WriteLine("Target:");
                TestContext.WriteLine(targetStructure);
                TestContext.WriteLine();
                throw;
            }
        }

        private void AssertComparerSeesNoDifference(IComparerSettings settings)
        {
            var comparer = CreateComparer(settings);
            var commands = comparer.Compare().Statements.ToArray();
            try
            {
                Assert.That(commands, Is.Empty);
            }
            catch
            {
                TestContext.WriteLine("Differences:");
                foreach (var item in commands)
                {
                    TestContext.WriteLine(item);
                }
                TestContext.WriteLine();
                throw;
            }
        }

        private ScriptResult UpdateDatabase(IComparerSettings settings)
        {
            var comparer = CreateComparer(settings);
            var compareResult = comparer.Compare().Script;
            TestContext.WriteLine("Change script:");
            var commands = compareResult.AllStatements.ToArray();
            foreach (var item in commands)
            {
                TestContext.WriteLine(item);
            }
            TestContext.WriteLine();
            Helpers.Database.ExecuteScript(m_Version, Helpers.Database.DatabaseLocation.Target, commands);
            return compareResult;
        }

        internal abstract class CompareSource : IEnumerable<TestCaseData>
        {
            private const string Base = "Compare.ComparerTestsData";
            private const string IgnoreSuffix = "ignore";
            private const string DefaultVersion = "00";

            private TargetVersion m_Version;

            public CompareSource(TargetVersion version)
            {
                m_Version = version;
            }

            private IEnumerable<TestCaseData> Data =>
                TestCaseScripts()
                    .Where(x => x.version == null || x.version == m_Version.VersionSuffix())
                    .Select(x =>
                    {
                        var result = new TestCaseData($"{Base}.{x.name}");
                        result.SetName(x.name);
                        if (x.ignore)
                        {
                            result.Ignore(string.Empty);
                        }
                        return result;
                    });

            public IEnumerator<TestCaseData> GetEnumerator()
            {
                foreach (var item in Data)
                    yield return item;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

            private static IEnumerable<(string name, string version, bool ignore)> TestCaseScripts()
            {
                return typeof(ComparerTests).Assembly.GetManifestResourceNames()
                    .Select(resourceName => Regex.Match(resourceName, $@"\.{Regex.Escape(Base)}\.(?<name>.+?\..+?_(?<version>\d+)(_(?<ignore>{IgnoreSuffix}))?)(\.[Source|Target]+)?\.sql$", RegexOptions.CultureInvariant))
                    .Where(match => match.Success)
                    .Select(match => (name: match.Groups["name"].Value, version: match.Groups["version"].Value, ignore: match.Groups["ignore"].Value))
                    .Distinct()
                    .OrderBy(x => x.name)
                    .Select(x => (x.name, x.version == DefaultVersion ? null : x.version, x.ignore != string.Empty));
            }
        }
        public virtual void Compare(string dataName)
        {
            Parallel.Invoke(
                () => Helpers.Database.ExecuteScript(m_Version, Helpers.Database.DatabaseLocation.Source, $"{dataName}.Source.sql"),
                () => Helpers.Database.ExecuteScript(m_Version, Helpers.Database.DatabaseLocation.Target, $"{dataName}.Target.sql"));
            var settings = new ComparerSettings(m_Version);
            var compareResult = UpdateDatabase(settings);
            AssertNoDifferenceBetweenSourceAndTarget();
            AssertComparerSeesNoDifference(settings);
            ExecuteSpecificAsserts(dataName, compareResult);
        }

        [Test]
        public void Settings_IgnorePermissions_True()
        {
            Parallel.Invoke(
                () => Helpers.Database.ExecuteScript(m_Version, Helpers.Database.DatabaseLocation.Source, new[] { "create table test(c int);", "grant all on test to someuser;" }),
                () => Helpers.Database.ExecuteScript(m_Version, Helpers.Database.DatabaseLocation.Target, new[] { "create table test(c int);" }));
            var settings = new ComparerSettings(m_Version) { IgnorePermissions = true };
            var compareResult = UpdateDatabase(settings);
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
            var compareResult = UpdateDatabase(settings);
            Assert.That(compareResult.AllStatements.Count(), Is.GreaterThanOrEqualTo(1));
            Assert.That(compareResult.AllStatements.First(), Does.StartWith("GRANT "));
            AssertComparerSeesNoDifference(settings);
        }
    }

    [TestFixture]
    public class ComparerTests25 : ComparerTests
    {
        private const TargetVersion Version = TargetVersion.Version25;

        public ComparerTests25()
            : base(Version)
        { }

        internal class CompareSource25 : CompareSource
        {
            public CompareSource25()
                : base(Version)
            { }
        }
        [TestCaseSource(typeof(CompareSource25))]
        public override void Compare(string dataName)
        {
            base.Compare(dataName);
        }
    }

    [TestFixture]
    public class ComparerTests30 : ComparerTests
    {
        private const TargetVersion Version = TargetVersion.Version30;

        public ComparerTests30()
            : base(Version)
        { }
        internal class CompareSource30 : CompareSource
        {
            public CompareSource30()
                : base(Version)
            { }
        }
        [TestCaseSource(typeof(CompareSource30))]
        public override void Compare(string dataName)
        {
            base.Compare(dataName);
        }
    }
}
