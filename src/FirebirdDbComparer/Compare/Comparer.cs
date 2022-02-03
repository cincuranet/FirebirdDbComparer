using System;
using System.Collections.Generic;
using System.Linq;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Compare;

public sealed partial class Comparer : IComparer
{  
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
        var compare = CompareImpl().Select(x => (x.name, commandGroups: x.commandGroups.ToArray())).ToList();
        var script = ScriptResult.Create(compare.Select(x => (x.name, ScriptBuilder.Build(x.commandGroups))));
        var statements = compare.SelectMany(x => x.commandGroups.SelectMany(y => y.Commands).Select(y => y.ToString())).ToList().AsReadOnly();
        return new CompareResult(script, statements);
    }

    private IEnumerable<(string name, IEnumerable<CommandGroup> commandGroups)> CompareImpl()
    {
        IEnumerable<(Func<IMetadata, IMetadata, IComparerContext, IEnumerable<CommandGroup>> action, string name)> Actions()
        {
            yield return (Handle<IMetadataDatabase>(x => x.HandleDatabase), "DATABASE");
            yield return (Handle<IMetadataFunctions>(x => x.CreateLegacyFunctions), "UDFS (new)");
            yield return (Handle<IMetadataCollations>(x => x.CreateCollations), "COLLATIONS (new)");
            yield return (Handle<IMetadataCharacterSets>(x => x.AlterCharacterSets), "CHARACTER SETS (alter)");
            yield return (Handle<IMetadataRoles>(x => x.CreateRoles), "ROLES (new)");
            yield return (Handle<IMetadataRoles>(x => x.AlterRoles), "ROLES (alter)");
            yield return (Handle<IMetadataFields>(x => x.CreateDomains), "DOMAINS (new)");
            yield return (Handle<IMetadataFields>(x => x.AlterDomains), "DOMAINS (alter)");
            yield return (Handle<IMetadataGenerators>(x => x.CreateGenerators), "SEQUENCES (new)");
            yield return (Handle<IMetadataGenerators>(x => x.AlterGenerators), "SEQUENCES (alter)");
            yield return (Handle<IMetadataExceptions>(x => x.CreateExceptions), "EXCEPTIONS (new)");
            yield return (Handle<IMetadataExceptions>(x => x.AlterExceptions), "EXCEPTIONS (alter)");
            yield return (Handle<IMetadataFunctions>(x => x.CreateEmptyNewFunctions), "FUNCTIONS (shims for new)");
            yield return (Handle<IMetadataProcedures>(x => x.CreateEmptyNewProcedures), "PROCEDURES (shims for new)");
            yield return (Handle<IMetadataFunctions>(x => x.AlterNewFunctionsToEmptyBodyForAlteringOrDropping), "FUNCTIONS (shims for altering and dropping)");
            yield return (Handle<IMetadataProcedures>(x => x.AlterProceduresToEmptyBodyForAlteringOrDropping), "PROCEDURES (shims for altering and dropping)");
            yield return (Handle<IMetadataRelations>(x => x.CreateEmptyViews), "VIEWS (shims for new)");
            yield return (Handle<IMetadataRelations>(x => x.AlterViewsToEmptyBodyForAlteringOrDropping), "VIEWS (shims for altering or dropping)");
            yield return (Handle<IMetadataRelations>(x => x.CreateTablesWithEmpty), "TABLES (new, computed fields as shims)");
            yield return (Handle<IMetadataRelations>(x => x.AlterTablesAndToEmptyForAlteringOrDropping), "TABLES (alter, computed fields as shims for altering or dropping)");
            yield return (Handle<IMetadataFunctions>(x => x.AlterLegacyFunctions), "UDFS (alter)");
            yield return (Handle<IMetadataPackages>(x => x.CreateNewPackagesHeaders), "PACKAGES HEADERS (new)");
            yield return (Handle<IMetadataPackages>(x => x.AlterPackagesHeaders), "PACKAGES HEADERS (alter)");
            yield return (Handle<IMetadataRelations>(x => x.AlterCreatedOrAlteredTablesToFull), "TABLES (full)");
            yield return (Handle<IMetadataTriggers>(x => x.HandleTriggers), "TRIGGERS");
            yield return (Handle<IMetadataFunctions>(x => x.AlterNewFunctionsToFullBody), "FUNCTIONS (full)");
            yield return (Handle<IMetadataProcedures>(x => x.AlterProceduresToFullBody), "PROCEDURES (full)");
            yield return (Handle<IMetadataRelations>(x => x.AlterViewsToFullBody), "VIEWS (full)");
            yield return (Handle<IMetadataPackages>(x => x.CreateNewPackagesBodies), "PACKAGES BODIES (new)");
            yield return (Handle<IMetadataPackages>(x => x.AlterPackagesBodies), "PACKAGES BODIES (alter)");
            yield return (Handle<IMetadataRelations>(x => x.HandleTableFieldsPositions), "FIELD POSITIONS");
            yield return (Handle<IMetadataIndices>(x => x.DropIndices), "INDICES (drop)");
            yield return (Handle<IMetadataConstraints>(x => x.HandleConstraints), "CONSTRAINTS");
            yield return (Handle<IMetadataIndices>(x => x.CreateAlterRecreateIndices), "INDICES (new, alter)");
            yield return (ProcessDeferredColumnsToDrop(), "DROP COLUMNS");
            yield return (Handle<IMetadataRelations>(x => x.DropTables), "TABLES (drop)");
            yield return (Handle<IMetadataRelations>(x => x.DropViews), "VIEWS (drop)");
            yield return (Handle<IMetadataProcedures>(x => x.DropProcedures), "PROCEDURES (drop)");
            yield return (Handle<IMetadataFunctions>(x => x.DropNewFunctions), "FUNCTIONS (drop)");
            yield return (Handle<IMetadataPackages>(x => x.DropPackages), "PACKAGES (drop)");
            yield return (Handle<IMetadataExceptions>(x => x.DropExceptions), "EXCEPTIONS (drop)");
            yield return (Handle<IMetadataGenerators>(x => x.DropGenerators), "SEQUENCES (drop)");
            yield return (Handle<IMetadataFields>(x => x.DropDomains), "DOMAINS (drop)");
            yield return (Handle<IMetadataRoles>(x => x.DropRoles), "ROLES (drop)");
            yield return (Handle<IMetadataCollations>(x => x.DropCollations), "COLLATIONS (drop)");
            yield return (Handle<IMetadataFunctions>(x => x.DropLegacyFunctions), "UDFS (drop)");
            yield return (Handle<IMetadataUserPrivileges>(x => x.HandleUserPrivileges), "GRANTS");
            yield return (HandleSupports<ISupportsComment>(), "COMMENTS");
        };

        return Actions().Select(x => (x.name, x.action(SourceMetadata, TargetMetadata, ComparerContext)));
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
