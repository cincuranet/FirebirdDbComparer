using System;
using System.Collections.Generic;
using System.Diagnostics;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Compare;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Elements
{
    [DebuggerDisplay("{FunctionNameKey}.{ArgumentName}")]
    public sealed class FunctionArgument : SqlElement<FunctionArgument>, IDataType, IHasCollation, IHasNullable, IHasSystemFlag, IHasDescription, IHasDefaultSource, IUsesField
    {
        private static readonly EquatableProperty<FunctionArgument>[] s_EquatableProperties =
        {
            new EquatableProperty<FunctionArgument>(x => x.FunctionName, nameof(FunctionName)),
            new EquatableProperty<FunctionArgument>(x => x.ArgumentPosition, nameof(ArgumentPosition)),
            new EquatableProperty<FunctionArgument>(x => x.Mechanism, nameof(Mechanism)),
            new EquatableProperty<FunctionArgument>(x => x.FieldLength, nameof(FieldLength)),
            new EquatableProperty<FunctionArgument>(x => x.FieldType, nameof(FieldType)),
            new EquatableProperty<FunctionArgument>(x => x.FieldSubType, nameof(FieldSubType)),
            new EquatableProperty<FunctionArgument>(x => x.FieldScale, nameof(FieldScale)),
            new EquatableProperty<FunctionArgument>(x => x.CharacterSetId, nameof(CharacterSetId)),
            new EquatableProperty<FunctionArgument>(x => x.FieldPrecision, nameof(FieldPrecision)),
            new EquatableProperty<FunctionArgument>(x => x.PackageName, nameof(PackageName)),
            new EquatableProperty<FunctionArgument>(x => x.ArgumentName, nameof(ArgumentName)),
            new EquatableProperty<FunctionArgument>(x => x.Field, nameof(Field)),
            new EquatableProperty<FunctionArgument>(x => x.DefaultSource, nameof(DefaultSource)),
            new EquatableProperty<FunctionArgument>(x => x._EqualityCollationId, nameof(CollationId)),
            new EquatableProperty<FunctionArgument>(x => x.Nullable, nameof(Nullable)),
            new EquatableProperty<FunctionArgument>(x => x.ArgumentMechanismLegacyStyle, nameof(ArgumentMechanismLegacyStyle)),
            new EquatableProperty<FunctionArgument>(x => x.FieldName, nameof(FieldName)),
            new EquatableProperty<FunctionArgument>(x => x.RelationName, nameof(RelationName)),
            new EquatableProperty<FunctionArgument>(x => x.SystemFlag, nameof(SystemFlag))
        };

        public FunctionArgument(ISqlHelper sqlHelper)
            : base(sqlHelper)
        { }

        public Identifier FunctionNameKey { get; private set; }
        public Identifier FunctionName { get; private set; }
        public int ArgumentPosition { get; private set; }
        public int Mechanism { get; private set; }
        public FunctionArgumentMechanism MechanismMechanism => (FunctionArgumentMechanism)Math.Abs(Mechanism);
        public bool MechanismFreeIt => Mechanism < 0;
        public Identifier PackageName { get; private set; }
        public Identifier ArgumentName { get; private set; }
        public Identifier FieldSource { get; private set; }
        public FunctionArgumentMechanism ArgumentMechanismLegacyStyle { get; private set; }
        public ProcedureParameterMechanism ArgumentMechanismNewStyle => (ProcedureParameterMechanism)ArgumentMechanismLegacyStyle;
        public Identifier FieldName { get; private set; }
        public Identifier RelationName { get; private set; }
        public int? FieldLength { get; private set; }
        public FieldType FieldType { get; private set; }
        public int? FieldSubType { get; private set; }
        public int? FieldScale { get; private set; }
        public int? CharacterSetId { get; private set; }
        public int? FieldPrecision { get; private set; }
        public int? CharacterLength { get; private set; }
        // should not matter, but CORE-6081
        internal int? _EqualityCollationId => !Function.IsLegacy ? CollationId : null;
        public int? CollationId { get; private set; }
        public int? SegmentSize => null;
        public DatabaseStringOrdinal DefaultSource { get; private set; }
        public DatabaseStringOrdinal Description { get; private set; }
        public bool Nullable { get; private set; }
        public SystemFlagType SystemFlag { get; private set; }

        public Function Function { get; set; }
        public Field Field { get; set; }
        public RelationField RelationField { get; set; }
        public Relation Relation { get; set; }
        public CharacterSet CharacterSet { get; set; }
        public Collation Collation { get; set; }
        public Package Package { get; set; }

        protected override FunctionArgument Self => this;

        protected override EquatableProperty<FunctionArgument>[] EquatableProperties => s_EquatableProperties;

        public static FunctionArgument CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values)
        {
            var result =
                new FunctionArgument(sqlHelper)
                {
                    FunctionName = new Identifier(sqlHelper, values["RDB$FUNCTION_NAME"].DbValueToString()),
                    ArgumentPosition = values["RDB$ARGUMENT_POSITION"].DbValueToInt32().GetValueOrDefault(),
                    Mechanism = values["RDB$MECHANISM"].DbValueToInt32().GetValueOrDefault(),
                    FieldType = (FieldType)values["RDB$FIELD_TYPE"].DbValueToInt32().GetValueOrDefault(),
                    FieldScale = values["RDB$FIELD_SCALE"].DbValueToInt32(),
                    FieldLength = values["RDB$FIELD_LENGTH"].DbValueToInt32(),
                    FieldSubType = values["RDB$FIELD_SUB_TYPE"].DbValueToInt32(),
                    CharacterSetId = values["RDB$CHARACTER_SET_ID"].DbValueToInt32(),
                    FieldPrecision = values["RDB$FIELD_PRECISION"].DbValueToInt32(),
                    CharacterLength = values["RDB$CHARACTER_LENGTH"].DbValueToInt32()
                };
            result.FunctionNameKey = result.FunctionName;

            if (sqlHelper.TargetVersion.AtLeast30())
            {
                result.PackageName = new Identifier(sqlHelper, values["RDB$PACKAGE_NAME"].DbValueToString());
                result.ArgumentName = new Identifier(sqlHelper, values["RDB$ARGUMENT_NAME"].DbValueToString());
                result.FieldSource = new Identifier(sqlHelper, values["RDB$FIELD_SOURCE"].DbValueToString());
                result.DefaultSource = values["RDB$DEFAULT_SOURCE"].DbValueToString();
                result.CollationId = values["RDB$COLLATION_ID"].DbValueToInt32();
                result.Nullable = values["RDB$NULL_FLAG"].DbValueToNullableFlag();
                result.ArgumentMechanismLegacyStyle = (FunctionArgumentMechanism)values["RDB$ARGUMENT_MECHANISM"].DbValueToInt32().GetValueOrDefault();
                result.FieldName = new Identifier(sqlHelper, values["RDB$FIELD_NAME"].DbValueToString());
                result.RelationName = new Identifier(sqlHelper, values["RDB$RELATION_NAME"].DbValueToString());
                result.SystemFlag = (SystemFlagType)values["RDB$SYSTEM_FLAG"].DbValueToInt32().GetValueOrDefault();
                result.Description = values["RDB$DESCRIPTION"].DbValueToString();

                result.FunctionNameKey = new Identifier(sqlHelper, result.FunctionName.ToString(), result.PackageName.ToString());
            }

            return result;
        }
    }
}
