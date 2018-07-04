using System;
using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.DatabaseObjects.Elements;
using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations
{
    public class MetadataFunctions25 : DatabaseObject, IMetadataFunctions, ISupportsComment
    {
        private IDictionary<FunctionArgumentKey, FunctionArgument> m_FunctionArguments;
        private IDictionary<Identifier, Function> m_FunctionsByName;

        public MetadataFunctions25(IMetadata metadata, ISqlHelper sqlHelper)
            : base(metadata, sqlHelper)
        { }

        protected virtual string FunctionCommandText => @"
select trim(F.RDB$FUNCTION_NAME) as RDB$FUNCTION_NAME,
       F.RDB$DESCRIPTION,
       F.RDB$MODULE_NAME,
       trim(F.RDB$ENTRYPOINT) as RDB$ENTRYPOINT,
       F.RDB$RETURN_ARGUMENT,
       F.RDB$SYSTEM_FLAG
from RDB$FUNCTIONS F";

        // RDB$FIELD_SUB_TYPE: weird discrepancy in some databases
        // RDB$FIELD_SCALE: weird discrepancy in some databases
        // RDB$FIELD_PRECISION: CORE-5550
        protected virtual string FunctionArgumentCommandText => @"
select trim(FA.RDB$FUNCTION_NAME) as RDB$FUNCTION_NAME,
       FA.RDB$ARGUMENT_POSITION,
       FA.RDB$MECHANISM,
       FA.RDB$FIELD_TYPE,
       coalesce(FA.RDB$FIELD_SCALE, 0) as RDB$FIELD_SCALE,
       FA.RDB$FIELD_LENGTH,
       coalesce(FA.RDB$FIELD_SUB_TYPE, 0) as RDB$FIELD_SUB_TYPE,
       FA.RDB$CHARACTER_SET_ID,
       coalesce(FA.RDB$FIELD_PRECISION, 0) as RDB$FIELD_PRECISION,
       FA.RDB$CHARACTER_LENGTH
from RDB$FUNCTION_ARGUMENTS FA";

        public IDictionary<FunctionArgumentKey, FunctionArgument> FunctionArguments => m_FunctionArguments;

        public virtual IDictionary<Identifier, Function> FunctionsByName => m_FunctionsByName;

        public virtual IDictionary<Identifier, Function> LegacyFunctionsByName => FunctionsByName;

        public virtual IDictionary<Identifier, Function> NewFunctionsByName => null;

        public override void Initialize()
        {
            m_FunctionArguments =
                Execute(FunctionArgumentCommandText)
                    .Select(o => FunctionArgument.CreateFrom(SqlHelper, o))
                    .ToDictionary(x => new FunctionArgumentKey(x.FunctionName, x.ArgumentPosition));
            var functionArguments = m_FunctionArguments.Values.ToLookup(x => x.FunctionName);

            m_FunctionsByName =
                Execute(FunctionCommandText)
                    .Select(o => Function.CreateFrom(SqlHelper, o, functionArguments))
                    .ToDictionary(x => x.FunctionNameKey);
        }

        public override void FinishInitialization()
        {
            foreach (var functionArgument in FunctionArguments.Values)
            {
                functionArgument.Function = FunctionsByName[functionArgument.FunctionNameKey];
                if (functionArgument.CharacterSetId != null)
                {
                    functionArgument.CharacterSet =
                        Metadata
                            .MetadataCharacterSets
                            .CharacterSetsById[functionArgument.CharacterSetId.Value];
                }
            }
        }

        public IEnumerable<CommandGroup> CreateLegacyFunctions(IMetadata other, IComparerContext context)
        {
            return FilterSystemFlagUser(LegacyFunctionsByName.Values)
                .Where(f => !other.MetadataFunctions.LegacyFunctionsByName.ContainsKey(f.FunctionNameKey))
                .Select(f => new CommandGroup().Append(f.Create(Metadata, other, context)));
        }

        public IEnumerable<CommandGroup> DropLegacyFunctions(IMetadata other, IComparerContext context)
        {
            return FilterSystemFlagUser(other.MetadataFunctions.LegacyFunctionsByName.Values)
                .Where(f => !LegacyFunctionsByName.ContainsKey(f.FunctionNameKey))
                .Select(f => new CommandGroup().Append(f.Drop(Metadata, other, context)));
        }

        public virtual IEnumerable<CommandGroup> CreateEmptyNewFunctions(IMetadata other, IComparerContext context)
        {
            yield break;
        }

        public virtual IEnumerable<CommandGroup> AlterNewFunctionsToFullBody(IMetadata other, IComparerContext context)
        {
            yield break;
        }

        public virtual IEnumerable<CommandGroup> AlterNewFunctionsToEmptyBodyForAlteringOrDropping(IMetadata other, IComparerContext context)
        {
            yield break;
        }

        public virtual IEnumerable<CommandGroup> DropNewFunctions(IMetadata other, IComparerContext context)
        {
            yield break;
        }

        IEnumerable<CommandGroup> ISupportsComment.Handle(IMetadata other, IComparerContext context)
        {
            var result = new CommandGroup().Append(HandleComment(FunctionsByName, other.MetadataFunctions.FunctionsByName, x => x.FunctionNameKey, "EXTERNAL FUNCTION", x => new[] { x.FunctionName }, context));
            if (!result.IsEmpty)
            {
                yield return result;
            }
        }
    }
}
