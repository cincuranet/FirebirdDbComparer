using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.IoC;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Compare
{
    public sealed class Comparer : IComparer
    {
        public static Comparer ForTwoDatabases(IComparerSettings settings, string sourceConnectionString, string targetConnectionString)
        {
            var container = Bootstrapper.BootstrapContainerDefault(settings);
            var metadataFactory = container.Resolve<IMetadataFactory>();
            var sourceMetadata = metadataFactory.Create(sourceConnectionString);
            var targetMetadata = metadataFactory.Create(targetConnectionString);
            Parallel.Invoke(
                sourceMetadata.Initialize,
                targetMetadata.Initialize);
            return (Comparer)container.Resolve<IComparerFactory>().Create(settings, sourceMetadata, targetMetadata);
        }

        public Comparer(ScriptBuilder scriptBuilder, IComparerContext comparerContext, IMetadata sourceMetadata, IMetadata targetMetadata)
        {
            ScriptBuilder = scriptBuilder ?? throw new ArgumentNullException(nameof(scriptBuilder));
            ComparerContext = comparerContext ?? throw new ArgumentNullException(nameof(comparerContext));
            SourceMetadata = sourceMetadata ?? throw new ArgumentNullException(nameof(sourceMetadata));
            TargetMetadata = targetMetadata ?? throw new ArgumentNullException(nameof(targetMetadata));
        }

        public ScriptBuilder ScriptBuilder { get; }
        public IComparerContext ComparerContext { get; }
        public IMetadata SourceMetadata { get; }
        public IMetadata TargetMetadata { get; }

        public CompareResult Compare()
        {
            var compare = CompareImpl().Select(x => Tuple.Create(x.Item1, x.Item2.ToArray())).ToArray();
            var script = ScriptResult.Create(compare.Select(x => Tuple.Create(x.Item1, ScriptBuilder.Build(x.Item2))));
            var statements = compare.SelectMany(x => x.Item2.SelectMany(y => y.Commands).Select(y => y.ToString())).ToList().AsReadOnly();
            return new CompareResult(script, statements);
        }

        private IEnumerable<Tuple<string, IEnumerable<CommandGroup>>> CompareImpl()
        {
            var actions =
                new[]
                {
                    Tuple.Create(Handle<IMetadataDatabase>(x => x.HandleDatabase), "DATABASE"),
                    Tuple.Create(Handle<IMetadataFunctions>(x => x.CreateLegacyFunctions), "UDFS (new)"),
                    Tuple.Create(Handle<IMetadataCollations>(x => x.CreateCollations), "COLLATIONS (new)"),
                    Tuple.Create(Handle<IMetadataCharacterSets>(x => x.AlterCharacterSets), "CHARACTER SETS (alter)"),
                    Tuple.Create(Handle<IMetadataRoles>(x => x.CreateRoles), "ROLES (new)"),
                    Tuple.Create(Handle<IMetadataRoles>(x => x.AlterRoles), "ROLES (alter)"),
                    Tuple.Create(Handle<IMetadataFields>(x => x.CreateDomains), "DOMAINS (new)"),
                    Tuple.Create(Handle<IMetadataFields>(x => x.AlterDomains), "DOMAINS (alter)"),
                    Tuple.Create(Handle<IMetadataGenerators>(x => x.CreateGenerators), "SEQUENCES (new)"),
                    Tuple.Create(Handle<IMetadataGenerators>(x => x.AlterGenerators), "SEQUENCES (alter)"),
                    Tuple.Create(Handle<IMetadataExceptions>(x => x.CreateExceptions), "EXCEPTIONS (new)"),
                    Tuple.Create(Handle<IMetadataExceptions>(x => x.AlterExceptions), "EXCEPTIONS (alter)"),
                    Tuple.Create(Handle<IMetadataPackages>(x => x.CreateNewPackagesHeaders), "PACKAGES HEADERS (new)"),
                    Tuple.Create(Handle<IMetadataFunctions>(x => x.CreateEmptyNewFunctions), "FUNCTIONS (shims for new)"),
                    Tuple.Create(Handle<IMetadataProcedures>(x => x.CreateEmptyNewProcedures), "PROCEDURES (shims for new)"),
                    Tuple.Create(Handle<IMetadataFunctions>(x => x.AlterNewFunctionsToEmptyBodyForAlteringOrDropping), "FUNCTIONS (shims for altering and dropping)"),
                    Tuple.Create(Handle<IMetadataProcedures>(x => x.AlterProceduresToEmptyBodyForAlteringOrDropping), "PROCEDURES (shims for altering and dropping)"),
                    Tuple.Create(Handle<IMetadataRelations>(x => x.CreateEmptyViews), "VIEWS (shims for new)"),
                    Tuple.Create(Handle<IMetadataRelations>(x => x.AlterViewsToEmptyBodyForAlteringOrDropping), "VIEWS (shims for altering or dropping)"),
                    Tuple.Create(Handle<IMetadataRelations>(x => x.CreateTablesWithEmpty), "TABLES (new, computed fields as shims)"),
                    Tuple.Create(Handle<IMetadataRelations>(x => x.AlterTablesAndToEmptyForAlteringOrDropping), "TABLES (alter, computed fields as shims for altering or dropping)"),
                    Tuple.Create(Handle<IMetadataRelations>(x => x.AlterCreatedOrAlteredTablesToFull), "TABLES (full)"),
                    Tuple.Create(Handle<IMetadataTriggers>(x => x.HandleTriggers), "TRIGGERS"),
                    Tuple.Create(Handle<IMetadataFunctions>(x => x.AlterNewFunctionsToFullBody), "FUNCTIONS (full)"),
                    Tuple.Create(Handle<IMetadataProcedures>(x => x.AlterProceduresToFullBody), "PROCEDURES (full)"),
                    Tuple.Create(Handle<IMetadataRelations>(x => x.AlterViewsToFullBody), "VIEWS (full)"),
                    Tuple.Create(Handle<IMetadataPackages>(x => x.CreateNewPackagesBodies), "PACKAGES BODIES (new)"),
                    Tuple.Create(Handle<IMetadataRelations>(x => x.HandleTableFieldsPositions), "FIELD POSITIONS"),
                    Tuple.Create(Handle<IMetadataIndices>(x => x.DropIndices), "INDICES (drop)"),
                    Tuple.Create(Handle<IMetadataConstraints>(x => x.HandleConstraints), "CONSTRAINTS"),
                    Tuple.Create(Handle<IMetadataIndices>(x => x.CreateAlterRecreateIndices), "INDICES (new, alter)"),
                    Tuple.Create(ProcessDeferredColumnsToDrop(), "DROP COLUMNS"),
                    Tuple.Create(Handle<IMetadataRelations>(x => x.DropTables), "TABLES (drop)"),
                    Tuple.Create(Handle<IMetadataRelations>(x => x.DropViews), "VIEWS (drop)"),
                    Tuple.Create(Handle<IMetadataProcedures>(x => x.DropProcedures), "PROCEDURES (drop)"),
                    Tuple.Create(Handle<IMetadataFunctions>(x => x.DropNewFunctions), "FUNCTIONS (drop)"),
                    Tuple.Create(Handle<IMetadataPackages>(x => x.DropFullPackages), "PACKAGES (drop)"),
                    Tuple.Create(Handle<IMetadataExceptions>(x => x.DropExceptions), "EXCEPTIONS (drop)"),
                    Tuple.Create(Handle<IMetadataGenerators>(x => x.DropGenerators), "SEQUENCES (drop)"),
                    Tuple.Create(Handle<IMetadataFields>(x => x.DropDomains), "DOMAINS (drop)"),
                    Tuple.Create(Handle<IMetadataRoles>(x => x.DropRoles), "ROLES (drop)"),
                    Tuple.Create(Handle<IMetadataCollations>(x => x.DropCollations), "COLLATIONS (drop)"),
                    Tuple.Create(Handle<IMetadataFunctions>(x => x.DropLegacyFunctions), "UDFS (drop)"),
                    Tuple.Create(Handle<IMetadataUserPrivileges>(x => x.HandleUserPrivileges), "GRANTS"),
                    Tuple.Create(HandleSupports<ISupportsComment>(), "COMMENTS")
                };

            return actions.Select(x => Tuple.Create(x.Item2, x.Item1(SourceMetadata, TargetMetadata, ComparerContext)));
        }

        private static Func<IMetadata, IMetadata, IComparerContext, IEnumerable<CommandGroup>> Handle<TInterface>(Func<TInterface, Func<IMetadata, IComparerContext, IEnumerable<CommandGroup>>> func)
        {
            return (s, t, c) => s.DatabaseObjects.OfType<TInterface>().Select(func).SelectMany(x => x(t, c));
        }

        private static Func<IMetadata, IMetadata, IComparerContext, IEnumerable<CommandGroup>> HandleSupports<TInterface>()
        {
            return Handle<TInterface>(x => (Func<IMetadata, IComparerContext, IEnumerable<CommandGroup>>)typeof(TInterface).GetMethod("Handle").CreateDelegate(typeof(Func<IMetadata, IComparerContext, IEnumerable<CommandGroup>>), x));
        }

        private static Func<IMetadata, IMetadata, IComparerContext, IEnumerable<CommandGroup>> ProcessDeferredColumnsToDrop()
        {
            return (_, __, c) => c.DeferredColumnsToDrop;
        }
    }
}
