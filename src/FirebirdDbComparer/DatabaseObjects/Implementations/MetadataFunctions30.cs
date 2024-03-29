using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Exceptions;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataFunctions30 : MetadataFunctions25, ISupportsComment
{
    private IDictionary<Identifier, Function> m_LegacyFunctionsByName;
    private IDictionary<Identifier, Function> m_NewNonPackageFunctionsByName;
    private IDictionary<Identifier, Function> m_NewPackageFunctionsByName;

    public MetadataFunctions30(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    protected override string FunctionCommandText => @"
select trim(F.RDB$FUNCTION_NAME) as RDB$FUNCTION_NAME,
       F.RDB$DESCRIPTION,
       F.RDB$MODULE_NAME,
       trim(F.RDB$ENTRYPOINT) as RDB$ENTRYPOINT,
       F.RDB$RETURN_ARGUMENT,
       F.RDB$SYSTEM_FLAG,
       trim(F.RDB$ENGINE_NAME) as RDB$ENGINE_NAME,
       trim(F.RDB$PACKAGE_NAME) RDB$PACKAGE_NAME,
       F.RDB$PRIVATE_FLAG,
       F.RDB$FUNCTION_SOURCE,
       F.RDB$OWNER_NAME,
       F.RDB$LEGACY_FLAG,
       F.RDB$DETERMINISTIC_FLAG
  from RDB$FUNCTIONS F";

    // RDB$FIELD_PRECISION: CORE-5550
    // CORE-5535 => weird iif for RDB$FIELD_SUB_TYPE
    protected override string FunctionArgumentCommandText => @"
select trim(FA.RDB$FUNCTION_NAME) as RDB$FUNCTION_NAME,
       FA.RDB$ARGUMENT_POSITION,
       FA.RDB$MECHANISM,
       FA.RDB$FIELD_TYPE,
       coalesce(FA.RDB$FIELD_SCALE, 0) as RDB$FIELD_SCALE,
       FA.RDB$FIELD_LENGTH,
       iif(FA.RDB$FIELD_TYPE = 261, coalesce(FA.RDB$FIELD_SUB_TYPE, 0), 0) as RDB$FIELD_SUB_TYPE,
       FA.RDB$CHARACTER_SET_ID,
       coalesce(FA.RDB$FIELD_PRECISION, 0) as RDB$FIELD_PRECISION,
       FA.RDB$CHARACTER_LENGTH,
       trim(FA.RDB$PACKAGE_NAME) as RDB$PACKAGE_NAME,
       trim(FA.RDB$ARGUMENT_NAME) as RDB$ARGUMENT_NAME,
       trim(FA.RDB$FIELD_SOURCE) as RDB$FIELD_SOURCE,
       FA.RDB$DEFAULT_SOURCE,
       FA.RDB$COLLATION_ID,
       FA.RDB$NULL_FLAG,
       FA.RDB$ARGUMENT_MECHANISM,
       trim(FA.RDB$FIELD_NAME) as RDB$FIELD_NAME,
       trim(FA.RDB$RELATION_NAME) as RDB$RELATION_NAME,
       FA.RDB$SYSTEM_FLAG,
       FA.RDB$DESCRIPTION
  from RDB$FUNCTION_ARGUMENTS FA";

    public override IDictionary<Identifier, Function> LegacyFunctionsByName => m_LegacyFunctionsByName;

    public override IDictionary<Identifier, Function> NewNonPackageFunctionsByName => m_NewNonPackageFunctionsByName;
    public override IDictionary<Identifier, Function> NewPackageFunctionsByName => m_NewPackageFunctionsByName;

    public override void FinishInitialization()
    {
        base.FinishInitialization();

        m_LegacyFunctionsByName = FunctionsByName.Values
            .Where(x => x.IsLegacy)
            .ToDictionary(x => x.FunctionNameKey, x => x);

        m_NewNonPackageFunctionsByName = FunctionsByName.Values
            .Where(x => !x.IsLegacy)
            .Where(x => x.PackageName == null)
            .ToDictionary(x => x.FunctionNameKey, x => x);
        m_NewPackageFunctionsByName = FunctionsByName.Values
            .Where(x => !x.IsLegacy)
            .Where(x => x.PackageName != null)
            .ToDictionary(x => x.FunctionNameKey, x => x);

        foreach (var functionArgument in FunctionArguments.Values)
        {
            if (functionArgument.FieldSource != null)
            {
                functionArgument.Field =
                    Metadata
                        .MetadataFields
                        .Fields[functionArgument.FieldSource];
                if (functionArgument.CollationId != null && functionArgument.Field.CharacterSetId != null)
                {
                    functionArgument.Collation =
                        Metadata
                            .MetadataCollations
                            .CollationsByKey[new CollationKey((int)functionArgument.Field.CharacterSetId, (int)functionArgument.CollationId)];
                }
            }
            if (functionArgument.FieldName != null && functionArgument.RelationName != null)
            {
                functionArgument.RelationField =
                    Metadata
                        .MetadataRelations
                        .RelationFields[new RelationFieldKey(functionArgument.RelationName, functionArgument.FieldName)];
                functionArgument.Relation =
                    Metadata
                        .MetadataRelations
                        .Relations[functionArgument.RelationName];
            }
            if (functionArgument.CharacterSetId != null)
            {
                functionArgument.CharacterSet =
                    Metadata
                        .MetadataCharacterSets
                        .CharacterSetsById[(int)functionArgument.CharacterSetId];
            }
            if (functionArgument.PackageName != null)
            {
                functionArgument.Package =
                    Metadata
                        .MetadataPackages
                        .PackagesByName[functionArgument.PackageName];
            }

        }

        foreach (var function in FunctionsByName.Values)
        {
            if (function.PackageName != null)
            {
                function.Package = Metadata.MetadataPackages.PackagesByName[function.PackageName];
            }
        }
    }

    public override IEnumerable<CommandGroup> CreateEmptyNewFunctions(IMetadata other, IComparerContext context)
    {
        return FilterNewNewFunctions(other)
            .Select(f => new CommandGroup().Append(WrapActionWithEmptyBody(f.Create)(Metadata, other, context)));
    }

    public override IEnumerable<CommandGroup> AlterNewFunctionsToFullBody(IMetadata other, IComparerContext context)
    {
        return FilterNewNewFunctions(other).Concat(FilterNewFunctionsToBeAltered(other))
            .Select(f => new CommandGroup().Append(f.Alter(Metadata, other, context)))
            .Where(x => !x.IsEmpty);
    }

    public override IEnumerable<CommandGroup> AlterNewFunctionsToEmptyBodyForAlteringOrDropping(IMetadata other, IComparerContext context)
    {
        return FilterNewFunctionsToBeDropped(other).Concat(FilterNewFunctionsToBeAltered(other))
            .Select(f => new CommandGroup().Append(WrapActionWithEmptyBody(f.Alter)(Metadata, other, context)))
            .Where(x => !x.IsEmpty);
    }

    public override IEnumerable<CommandGroup> DropNewFunctions(IMetadata other, IComparerContext context)
    {
        return FilterNewFunctionsToBeDropped(other)
            .Select(f => new CommandGroup().Append(f.Drop(Metadata, other, context)));
    }

    private IEnumerable<Function> FilterNewNewFunctions(IMetadata other)
    {
        return FilterSystemFlagUser(NewNonPackageFunctionsByName.Values)
            .Where(f =>
            {
                other.MetadataFunctions.FunctionsByName.TryGetValue(f.FunctionNameKey, out var otherFunction);
                if (otherFunction == null || otherFunction.LegacyFlag != f.LegacyFlag)
                {
                    if (otherFunction != null && otherFunction.LegacyFlag != f.LegacyFlag)
                    {
                        throw new CrossTypesOfSameObjectTypesException();
                    }
                    return true;
                }
                return false;
            });
    }

    private IEnumerable<Function> FilterNewFunctionsToBeDropped(IMetadata other)
    {
        return FilterSystemFlagUser(other.MetadataFunctions.NewNonPackageFunctionsByName.Values)
            .Where(f => !NewNonPackageFunctionsByName.ContainsKey(f.FunctionNameKey));
    }

    private IEnumerable<Function> FilterNewFunctionsToBeAltered(IMetadata other)
    {
        return FilterSystemFlagUser(NewNonPackageFunctionsByName.Values)
            .Where(f => other.MetadataFunctions.NewNonPackageFunctionsByName.TryGetValue(f.FunctionNameKey, out var otherFunction) && otherFunction != f);
    }

    IEnumerable<CommandGroup> ISupportsComment.Handle(IMetadata other, IComparerContext context)
    {
        static IEnumerable<Identifier> Name(Function x) => x.PackageName != null ? new[] { x.PackageName, x.FunctionName } : new[] { x.FunctionName };
        IEnumerable<Command> Nested(Function x) => HandleCommentNested(x.FunctionArguments.OrderBy(x => x.ArgumentPosition), other.MetadataFunctions.FunctionArguments, y => new FunctionArgumentKey(x.FunctionNameKey, y.ArgumentPosition, y.ArgumentName), Name(x), "PARAMETER", y => new[] { y.ArgumentName }, context);

        var result = new CommandGroup()
            .Append(HandleComment(LegacyFunctionsByName, other.MetadataFunctions.LegacyFunctionsByName, x => x.FunctionNameKey, "EXTERNAL FUNCTION", x => new[] { x.FunctionName }, context))
            .Append(HandleComment(NewNonPackageFunctionsByName, other.MetadataFunctions.NewNonPackageFunctionsByName, x => x.FunctionNameKey, "FUNCTION", Name, context, Nested))
            .Append(HandleComment(NewPackageFunctionsByName, other.MetadataFunctions.NewPackageFunctionsByName, x => x.FunctionNameKey, "FUNCTION", Name, context, Nested));
        if (!result.IsEmpty)
        {
            yield return result;
        }
    }
}
