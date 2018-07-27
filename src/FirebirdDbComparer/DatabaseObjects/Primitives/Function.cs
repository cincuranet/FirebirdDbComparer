using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Compare;
using FirebirdDbComparer.DatabaseObjects.Elements;
using FirebirdDbComparer.Exceptions;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Primitives
{
    [DebuggerDisplay("{FunctionName}")]
    public sealed class Function : Primitive<Function>, IHasSystemFlag, IHasDescription, IHasPackage
    {
        private static readonly EquatableProperty<Function>[] s_EquatableProperties =
        {
            new EquatableProperty<Function>(x => x.FunctionName, nameof(FunctionName)),
            new EquatableProperty<Function>(x => x.ModuleName, nameof(ModuleName)),
            new EquatableProperty<Function>(x => x.EntryPoint, nameof(EntryPoint)),
            new EquatableProperty<Function>(x => x.ReturnArgument, nameof(ReturnArgument)),
            new EquatableProperty<Function>(x => x.FunctionArguments, nameof(FunctionArguments)),
            new EquatableProperty<Function>(x => x.SystemFlag, nameof(SystemFlag)),
            new EquatableProperty<Function>(x => x.EngineName, nameof(EngineName)),
            new EquatableProperty<Function>(x => x.PackageName, nameof(PackageName)),
            new EquatableProperty<Function>(x => x.PrivateFlag, nameof(PrivateFlag)),
            new EquatableProperty<Function>(x => x.FunctionSource, nameof(FunctionSource)),
            new EquatableProperty<Function>(x => x.Owner, nameof(Owner)),
            new EquatableProperty<Function>(x => x.LegacyFlag, nameof(LegacyFlag)),
            new EquatableProperty<Function>(x => x.DeterministicFlag, nameof(DeterministicFlag))
        };

        public Function(ISqlHelper sqlHelper)
            : base(sqlHelper)
        { }

        public Identifier FunctionNameKey { get; private set; }
        public Identifier FunctionName { get; private set; }
        public DatabaseStringOrdinal ModuleName { get; private set; }
        public DatabaseStringOrdinal EntryPoint { get; private set; }
        public int ReturnArgument { get; private set; }
        public IList<FunctionArgument> FunctionArguments { get; private set; }
        public Identifier EngineName { get; private set; }
        public Identifier PackageName { get; private set; }
        public PrivateFlagType PrivateFlag { get; private set; }
        public DatabaseStringOrdinal FunctionSource { get; private set; }
        public Identifier Owner { get; private set; }
        public LegacyFlagType? LegacyFlag { get; private set; }
        public DeterministicFlagType DeterministicFlag { get; private set; }
        public DatabaseStringOrdinal Description { get; private set; }
        public SystemFlagType SystemFlag { get; private set; }
        public Package Package { get; set; }

        internal bool IsLegacy => LegacyFlag == null || LegacyFlag == LegacyFlagType.LEGACY_STYLE;

        protected override Function Self => this;

        protected override EquatableProperty<Function>[] EquatableProperties => s_EquatableProperties;

        protected override IEnumerable<Command> OnCreate(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            if (IsLegacy)
            {
                var command = new Command();
                command.Append($"DECLARE EXTERNAL FUNCTION {FunctionName.AsSqlIndentifier()}");
                var inputs =
                    FunctionArguments
                        .Where(x => x.ArgumentPosition != ReturnArgument)
                        .OrderBy(x => x.ArgumentPosition)
                        .Select(CreateLegacyArgumentDefinition);
                command.Append($" {string.Join(", ", inputs)}");
                command.AppendLine();
                var @return = FunctionArguments.First(x => x.ArgumentPosition == ReturnArgument);
                command.Append($"RETURNS{(ReturnArgument != 0 ? " PARAMETER" : string.Empty)} {CreateLegacyArgumentDefinition(@return)}");
                command.AppendLine();
                command.Append($"ENTRY_POINT '{SqlHelper.DoubleSingleQuotes(EntryPoint)}' MODULE_NAME '{SqlHelper.DoubleSingleQuotes(ModuleName)}'");
                yield return command;
            }
            else
            {
                var command = new PSqlCommand();
                command.Append("CREATE OR ALTER FUNCTION");
                if (PackageName != null)
                {
                    command.Append($" {PackageName.AsSqlIndentifier()}.{FunctionName.AsSqlIndentifier()}");
                }
                else
                {
                    command.Append($" {FunctionName.AsSqlIndentifier()}");
                }
                var inputs =
                    FunctionArguments
                        .Where(x => x.ArgumentPosition != ReturnArgument)
                        .OrderBy(x => x.ArgumentPosition)
                        .Select(x => CreateNewArgumentDefinition(x, sourceMetadata, targetMetadata, context));
                var @return = FunctionArguments.First(x => x.ArgumentPosition == ReturnArgument);
                var output = CreateNewArgumentDefinition(@return, sourceMetadata, targetMetadata, context);
                command.Append($" ({string.Join(", ", inputs)}) RETURNS {output}");
                command.AppendLine();
                command.Append("AS");
                command.AppendLine();
                if (context.EmptyBodiesEnabled)
                {
                    command.Append("BEGIN");
                    command.AppendLine();
                    command.Append("END");
                }
                else
                {
                    command.Append(FunctionSource);
                }
                if (EntryPoint != null)
                {
                    command.AppendLine();
                    command.Append($"EXTERNAL NAME '{EntryPoint}'");
                }
                if (EngineName != null)
                {
                    command.AppendLine();
                    command.Append($"ENGINE {EngineName.AsSqlIndentifier()}");
                }
                yield return command;
            }
        }

        protected override IEnumerable<Command> OnDrop(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            var command = new Command();
            if (IsLegacy)
            {
                command.Append($"DROP EXTERNAL FUNCTION {FunctionName.AsSqlIndentifier()}");
            }
            else
            {
                command.Append($"DROP FUNCTION");
                if (PackageName != null)
                {
                    command.Append($" {PackageName.AsSqlIndentifier()}.{FunctionName.AsSqlIndentifier()}");
                }
                else
                {
                    command.Append($" {FunctionName.AsSqlIndentifier()}");
                }
            }
            yield return command;
        }

        protected override IEnumerable<Command> OnAlter(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            if (IsLegacy)
            {
                throw new NotSupportedOnFirebirdException($"Altering function is not supported ({FunctionName}).");
            }
            else
            {
                return OnCreate(sourceMetadata, targetMetadata, context);
            }
        }

        protected override Identifier OnPrimitiveTypeKeyObjectName() => FunctionNameKey;

        private string CreateLegacyArgumentDefinition(FunctionArgument argument)
        {
            var builder = new StringBuilder();
            builder.Append(SqlHelper.GetDataType(argument, default, default));
            switch (argument.MechanismMechanism)
            {
                case FunctionArgumentMechanism.BY_VALUE:
                    builder.Append(" BY VALUE");
                    break;
                case FunctionArgumentMechanism.BY_REFERENCE:
                    break;
                case FunctionArgumentMechanism.BY_REFERENCE_WITH_NULL:
                    builder.Append(" NULL");
                    break;
                case FunctionArgumentMechanism.BY_VMS_DESCRIPTOR:
                case FunctionArgumentMechanism.BY_ISC_DESCRIPTOR:
                    builder.Append(" BY DESCRIPTOR");
                    break;
            }
            if (argument.MechanismFreeIt)
            {
                builder.Append(" FREE_IT");
            }
            return builder.ToString();
        }

        private string CreateNewArgumentDefinition(FunctionArgument argument, IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            var builder = new StringBuilder();
            if (argument.ArgumentName != null)
            {
                builder.Append(argument.ArgumentName.AsSqlIndentifier());
                builder.Append(" ");
            }
            if (context.EmptyBodiesEnabled)
            {
                builder.Append(SqlHelper.GetDataType(argument.Field, sourceMetadata.MetadataCharacterSets.CharacterSetsById, sourceMetadata.MetadataDatabase.CharacterSet.CharacterSetId));
            }
            else
            {
                builder.Append(SqlHelper.GetDataType(argument, sourceMetadata.MetadataCharacterSets.CharacterSetsById, sourceMetadata.MetadataDatabase.CharacterSet.CharacterSetId));
            }
            var notNullClause = SqlHelper.HandleNullable(argument);
            if (notNullClause != null)
            {
                builder.Append(" ");
                builder.Append(notNullClause);
            }
            var collateClause = SqlHelper.HandleCollate(argument, sourceMetadata.MetadataCollations.CollationsByKey);
            if (collateClause != null)
            {
                builder.Append(" ");
                builder.Append(collateClause);
            }
            var defaultClause = SqlHelper.HandleDefault(argument);
            if (defaultClause != null)
            {
                builder.Append(" ");
                builder.Append(defaultClause);
            }
            return builder.ToString();
        }

        internal static Function CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values, ILookup<Identifier, FunctionArgument> functionArguments)
        {
            var result =
                new Function(sqlHelper)
                {
                    FunctionName = new Identifier(sqlHelper, values["RDB$FUNCTION_NAME"].DbValueToString()),
                    Description = values["RDB$DESCRIPTION"].DbValueToString(),
                    ModuleName = values["RDB$MODULE_NAME"].DbValueToString(),
                    EntryPoint = values["RDB$ENTRYPOINT"].DbValueToString(),
                    ReturnArgument = values["RDB$RETURN_ARGUMENT"].DbValueToInt32().GetValueOrDefault(),
                    SystemFlag = (SystemFlagType)values["RDB$SYSTEM_FLAG"].DbValueToInt32().GetValueOrDefault()
                };
            result.FunctionArguments = functionArguments[result.FunctionName].ToArray();
            result.FunctionNameKey = result.FunctionName;

            if (sqlHelper.TargetVersion.AtLeast30())
            {
                result.EngineName = new Identifier(sqlHelper, values["RDB$ENGINE_NAME"].DbValueToString());
                result.PackageName = new Identifier(sqlHelper, values["RDB$PACKAGE_NAME"].DbValueToString());
                result.PrivateFlag = (PrivateFlagType)values["RDB$PRIVATE_FLAG"].DbValueToInt32().GetValueOrDefault();
                result.FunctionSource = values["RDB$FUNCTION_SOURCE"].DbValueToString();
                result.Owner = new Identifier(sqlHelper, values["RDB$OWNER_NAME"].DbValueToString());
                result.LegacyFlag = (LegacyFlagType)values["RDB$LEGACY_FLAG"].DbValueToInt32().GetValueOrDefault();
                result.DeterministicFlag = (DeterministicFlagType)values["RDB$DETERMINISTIC_FLAG"].DbValueToInt32().GetValueOrDefault();

                result.FunctionNameKey = new Identifier(sqlHelper, $"{result.PackageName}.{result.FunctionName}");
            }

            return result;
        }
    }
}
