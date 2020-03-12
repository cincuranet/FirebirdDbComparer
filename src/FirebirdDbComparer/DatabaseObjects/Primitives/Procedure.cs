using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Compare;
using FirebirdDbComparer.DatabaseObjects.Elements;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Primitives
{
    [DebuggerDisplay("{FunctionNameKey}")]
    public sealed class Procedure : Primitive<Procedure>, IHasSystemFlag, IHasDescription, IHasPackage, IHasExternalEngine
    {
        private static readonly EquatableProperty<Procedure>[] s_EquatableProperties =
        {
            new EquatableProperty<Procedure>(x => x.ProcedureName, nameof(ProcedureName)),
            new EquatableProperty<Procedure>(x => x.ProcedureInputs, nameof(ProcedureInputs)),
            new EquatableProperty<Procedure>(x => x.ProcedureOutputs, nameof(ProcedureOutputs)),
            new EquatableProperty<Procedure>(x => x.ProcedureSource, nameof(ProcedureSource)),
            new EquatableProperty<Procedure>(x => x.OwnerName, nameof(OwnerName)),
            new EquatableProperty<Procedure>(x => x.ProcedureType, nameof(ProcedureType)),
            new EquatableProperty<Procedure>(x => x.SystemFlag, nameof(SystemFlag)),
            new EquatableProperty<Procedure>(x => x.ProcedureParameters, nameof(ProcedureParameters)),
            new EquatableProperty<Procedure>(x => x.EngineName, nameof(EngineName)),
            new EquatableProperty<Procedure>(x => x.EntryPoint, nameof(EntryPoint)),
            new EquatableProperty<Procedure>(x => x.PackageName, nameof(PackageName)),
            new EquatableProperty<Procedure>(x => x.PrivateFlag, nameof(PrivateFlag))
        };

        public Procedure(ISqlHelper sqlHelper)
            : base(sqlHelper)
        { }

        public Identifier ProcedureNameKey { get; private set; }
        public Identifier ProcedureName { get; private set; }
        public int ProcedureId { get; private set; }
        public int ProcedureInputs { get; private set; }
        public int ProcedureOutputs { get; private set; }
        public DatabaseStringOrdinal Description { get; private set; }
        public DatabaseStringOrdinal ProcedureSource { get; private set; }
        public DatabaseStringOrdinal OwnerName { get; private set; }
        public ProcedureProcedureType ProcedureType { get; private set; }
        public SystemFlagType SystemFlag { get; private set; }
        public Identifier EngineName { get; private set; }
        public DatabaseStringOrdinal EntryPoint { get; private set; }
        public Identifier PackageName { get; private set; }
        public PrivateFlagType PrivateFlag { get; private set; }
        public IList<ProcedureParameter> ProcedureParameters { get; set; }
        public Package Package { get; set; }

        protected override Procedure Self => this;

        protected override EquatableProperty<Procedure>[] EquatableProperties => s_EquatableProperties;

        protected override IEnumerable<Command> OnCreate(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            if (ProcedureType == ProcedureProcedureType.Legacy)
            {
                throw new NotSupportedException("Legacy type stored procedures are not supported.");
            }

            var command = SqlHelper.IsValidExternalEngine(this) ? new Command() : new PSqlCommand();
            command.Append($"CREATE OR ALTER PROCEDURE {ProcedureName.AsSqlIndentifier()}");
            if (ProcedureInputs > 0)
            {
                var inputs = ProcedureParameters
                    .Where(o => o.ParameterType == ProcedureParameterType.In)
                    .OrderBy(o => o.ParameterNumber);
                command.Append(" (");
                command.AppendLine();
                command.Append(CreateParametersDefinitions(inputs, sourceMetadata, targetMetadata, context));
                command.Append(")");
            }
            command.AppendLine();
            if (ProcedureOutputs > 0)
            {
                var outputs = ProcedureParameters
                    .Where(o => o.ParameterType == ProcedureParameterType.Out)
                    .OrderBy(o => o.ParameterNumber);
                command.Append("RETURNS (");
                command.AppendLine();
                command.Append(CreateParametersDefinitions(outputs, sourceMetadata, targetMetadata, context));
                command.Append(")");
                command.AppendLine();
            }
            if (SqlHelper.IsValidExternalEngine(this))
            {
                if (context.EmptyBodiesEnabled)
                {
                    yield break;
                }
                command.Append($"EXTERNAL NAME '{SqlHelper.DoubleSingleQuotes(EntryPoint)}'");
                command.AppendLine();
                command.Append($"ENGINE {EngineName.AsSqlIndentifier()}");
            }
            else
            {
                command.Append("AS");
                command.AppendLine();
                if (context.EmptyBodiesEnabled)
                {
                    command.Append("BEGIN");
                    command.AppendLine();
                    if (ProcedureType == ProcedureProcedureType.Selectable)
                    {
                        command.Append($"  SUSPEND{SqlHelper.Terminator}");
                        command.AppendLine();
                    }
                    command.Append("END");
                }
                else
                {
                    command.Append(ProcedureSource);
                }
            }
            yield return command;
        }

        protected override IEnumerable<Command> OnDrop(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            yield return new Command()
                .Append($"DROP PROCEDURE {ProcedureName.AsSqlIndentifier()}");
        }

        protected override IEnumerable<Command> OnAlter(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            return OnCreate(sourceMetadata, targetMetadata, context);
        }

        protected override Identifier OnPrimitiveTypeKeyObjectName() => ProcedureName;

        private string CreateParametersDefinitions(IEnumerable<ProcedureParameter> parameters, IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            var builder = new StringBuilder();
            builder.Append("  ");
            var definitions = parameters.Select(x => CreateParameterDefinition(x, targetMetadata, sourceMetadata, context));
            builder.Append(string.Join($",{Environment.NewLine}  ", definitions));
            return builder.ToString();
        }

        private string CreateParameterDefinition(ProcedureParameter parameter, IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            var builder = new StringBuilder();
            builder.Append(parameter.ParameterName.AsSqlIndentifier());
            builder.Append(" ");
            if (context.EmptyBodiesEnabled)
            {
                builder.Append(SqlHelper.GetDataType(parameter.Field, sourceMetadata.MetadataCharacterSets.CharacterSetsById, sourceMetadata.MetadataDatabase.CharacterSet.CharacterSetId));
            }
            else
            {
                builder.Append(SqlHelper.GetDataType(parameter, sourceMetadata.MetadataCharacterSets.CharacterSetsById, sourceMetadata.MetadataDatabase.CharacterSet.CharacterSetId));
            }
            var notNullClause = SqlHelper.HandleNullable(parameter);
            if (notNullClause != null)
            {
                builder.Append(" ");
                builder.Append(notNullClause);
            }
            var collateClause = SqlHelper.HandleCollate(parameter, sourceMetadata.MetadataCollations.CollationsByKey);
            if (collateClause != null)
            {
                builder.Append(" ");
                builder.Append(collateClause);
            }
            var defaultClause = SqlHelper.HandleDefault(parameter);
            if (defaultClause != null)
            {
                builder.Append(" ");
                builder.Append(defaultClause);
            }
            return builder.ToString();
        }

        internal static Procedure CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values, ILookup<Identifier, ProcedureParameter> procedureParameters)
        {
            var result =
                new Procedure(sqlHelper)
                {
                    ProcedureName = new Identifier(sqlHelper, values["RDB$PROCEDURE_NAME"].DbValueToString()),
                    ProcedureId = values["RDB$PROCEDURE_ID"].DbValueToInt32().GetValueOrDefault(),
                    ProcedureInputs = values["RDB$PROCEDURE_INPUTS"].DbValueToInt32().GetValueOrDefault(),
                    ProcedureOutputs = values["RDB$PROCEDURE_OUTPUTS"].DbValueToInt32().GetValueOrDefault(),
                    Description = values["RDB$DESCRIPTION"].DbValueToString(),
                    ProcedureSource = values["RDB$PROCEDURE_SOURCE"].DbValueToString(),
                    OwnerName = values["RDB$OWNER_NAME"].DbValueToString(),
                    ProcedureType = (ProcedureProcedureType)values["RDB$PROCEDURE_TYPE"].DbValueToInt32().GetValueOrDefault(),
                    SystemFlag = (SystemFlagType)values["RDB$SYSTEM_FLAG"].DbValueToInt32().GetValueOrDefault()
                };
            result.ProcedureNameKey = new Identifier(sqlHelper, result.ProcedureName);

            if (sqlHelper.TargetVersion.AtLeast30())
            {
                result.EngineName = new Identifier(sqlHelper, values["RDB$ENGINE_NAME"].DbValueToString());
                result.EntryPoint = values["RDB$ENTRYPOINT"].DbValueToString();
                result.PackageName = new Identifier(sqlHelper, values["RDB$PACKAGE_NAME"].DbValueToString());
                result.PrivateFlag = (PrivateFlagType)values["RDB$PRIVATE_FLAG"].DbValueToInt32().GetValueOrDefault();

                result.ProcedureNameKey = new Identifier(sqlHelper, result.PackageName, result.ProcedureName);
            }

            result.ProcedureParameters = procedureParameters[result.ProcedureNameKey].ToArray();

            return result;
        }
    }
}
