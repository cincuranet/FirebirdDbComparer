using System.Collections.Generic;
using System.Diagnostics;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Compare;
using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Elements
{
    [DebuggerDisplay("{ProcedureNameKey}.{ParameterName}")]
    public sealed class ProcedureParameter : SqlElement<ProcedureParameter>, IHasDescription, IHasDefaultSource, IHasNullable, IHasCollation, IUsesField, ITypeObjectNameKey
    {
        private static readonly EquatableProperty<ProcedureParameter>[] s_EquatableProperties =
        {
            new EquatableProperty<ProcedureParameter>(x => x.ParameterName, nameof(ParameterName)),
            new EquatableProperty<ProcedureParameter>(x => x.ProcedureName, nameof(ProcedureName)),
            new EquatableProperty<ProcedureParameter>(x => x.ParameterNumber, nameof(ParameterNumber)),
            new EquatableProperty<ProcedureParameter>(x => x.Field, nameof(Field)),
            new EquatableProperty<ProcedureParameter>(x => x.SystemFlag, nameof(SystemFlag)),
            new EquatableProperty<ProcedureParameter>(x => x.DefaultSource, nameof(DefaultSource)),
            new EquatableProperty<ProcedureParameter>(x => x.Collation, nameof(Collation)),
            new EquatableProperty<ProcedureParameter>(x => x.Nullable, nameof(Nullable)),
            new EquatableProperty<ProcedureParameter>(x => x.ParameterMechanism, nameof(ParameterMechanism)),
            new EquatableProperty<ProcedureParameter>(x => x.FieldName, nameof(FieldName)),
            new EquatableProperty<ProcedureParameter>(x => x.RelationName, nameof(RelationName)),
            new EquatableProperty<ProcedureParameter>(x => x.PackageName, nameof(PackageName))
        };

        private TypeObjectNameKey m_PrimitiveTypeKey;

        public ProcedureParameter(ISqlHelper sqlHelper)
            : base(sqlHelper)
        { }

        public Identifier ParameterName { get; private set; }
        public Identifier ProcedureNameKey { get; private set; }
        public Identifier ProcedureName { get; private set; }
        public int ParameterNumber { get; private set; }
        public ProcedureParameterType ParameterType { get; private set; }
        public Identifier FieldSource { get; private set; }
        public DatabaseStringOrdinal Description { get; private set; }
        public SystemFlagType SystemFlag { get; private set; }
        public DatabaseStringOrdinal DefaultSource { get; private set; }
        public int? CollationId { get; private set; }
        public bool Nullable { get; private set; }
        public ProcedureParameterMechanism ParameterMechanism { get; private set; }
        public Identifier FieldName { get; private set; }
        public Identifier RelationName { get; private set; }
        public Identifier PackageName { get; private set; }

        public Procedure Procedure { get; set; }
        public Field Field { get; set; }
        public Collation Collation { get; set; }
        public RelationField RelationField { get; set; }
        public Relation Relation { get; set; }
        public Package Package { get; set; }

        protected override ProcedureParameter Self => this;

        protected override EquatableProperty<ProcedureParameter>[] EquatableProperties => s_EquatableProperties;

        internal static ProcedureParameter CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values)
        {
            var result =
                new ProcedureParameter(sqlHelper)
                {
                    ParameterName = new Identifier(sqlHelper, values["RDB$PARAMETER_NAME"].DbValueToString()),
                    ProcedureName = new Identifier(sqlHelper, values["RDB$PROCEDURE_NAME"].DbValueToString()),
                    ParameterNumber = values["RDB$PARAMETER_NUMBER"].DbValueToInt32().GetValueOrDefault(),
                    ParameterType = (ProcedureParameterType)values["RDB$PARAMETER_TYPE"].DbValueToInt32().GetValueOrDefault(),
                    FieldSource = new Identifier(sqlHelper, values["RDB$FIELD_SOURCE"].DbValueToString()),
                    Description = values["RDB$DESCRIPTION"].DbValueToString(),
                    SystemFlag = (SystemFlagType)values["RDB$SYSTEM_FLAG"].DbValueToInt32().GetValueOrDefault(),
                    DefaultSource = values["RDB$DEFAULT_SOURCE"].DbValueToString(),
                    CollationId = values["RDB$COLLATION_ID"].DbValueToInt32(),
                    Nullable = values["RDB$NULL_FLAG"].DbValueToNullableFlag(),
                    ParameterMechanism = (ProcedureParameterMechanism)values["RDB$PARAMETER_MECHANISM"].DbValueToInt32().GetValueOrDefault(),
                    FieldName = new Identifier(sqlHelper, values["RDB$FIELD_NAME"].DbValueToString()),
                    RelationName = new Identifier(sqlHelper, values["RDB$RELATION_NAME"].DbValueToString())
                };
            result.ProcedureNameKey = result.ProcedureName;

            if (sqlHelper.TargetVersion.AtLeast30())
            {
                result.PackageName = new Identifier(sqlHelper, values["RDB$PACKAGE_NAME"].DbValueToString());

                result.ProcedureNameKey = new Identifier(sqlHelper, result.ProcedureName.ToString(), result.PackageName.ToString());
            }
            return result;
        }

        public TypeObjectNameKey TypeObjectNameKey => m_PrimitiveTypeKey ?? (m_PrimitiveTypeKey = new TypeObjectNameKey(GetType(), ParameterName));
    }
}
