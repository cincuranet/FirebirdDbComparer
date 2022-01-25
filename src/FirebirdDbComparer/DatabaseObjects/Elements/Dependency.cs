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
    [DebuggerDisplay("DependentName:{DependentName} DependedOnName:{DependedOnName}")]
    public sealed class Dependency : SqlElement<Dependency>, ITypeObjectNameKey
    {
        private static readonly EquatableProperty<Dependency>[] s_EquatableProperties =
        {
            new EquatableProperty<Dependency>(x => x.DependentName, nameof(DependentName)),
            new EquatableProperty<Dependency>(x => x.DependedOnName, nameof(DependedOnName)),
            new EquatableProperty<Dependency>(x => x.FieldName, nameof(FieldName)),
            new EquatableProperty<Dependency>(x => x.DependentType, nameof(DependentType)),
            new EquatableProperty<Dependency>(x => x.DependedOnType, nameof(DependedOnType)),
            new EquatableProperty<Dependency>(x => x.PackageName, nameof(PackageName))
        };

        private TypeObjectNameKey m_PrimitiveTypeKey;

        public Dependency(ISqlHelper sqlHelper)
            : base(sqlHelper)
        { }

        public Identifier DependentName { get; private set; }
        public Identifier DependedOnName { get; private set; }
        public Identifier FieldName { get; private set; }
        public ObjectType DependentType { get; private set; }
        public ObjectType DependedOnType { get; private set; }
        public Identifier PackageName { get; private set; }
        public Identifier DependentNameKey { get; private set; }
        public Identifier DependedOnNameKey { get; private set; }

        public Relation DependentRelation { get; set; }
        public Relation DependedOnRelation { get; set; }

        public Trigger DependentTrigger { get; set; }
        public Trigger DependedOnTrigger { get; set; }

        public Field DependentField { get; set; }
        public Field DependedOnField { get; set; }

        public Procedure DependentProcedure { get; set; }
        public Procedure DependedOnProcedure { get; set; }

        public DbException DependentException { get; set; }
        public DbException DependedOnException { get; set; }

        public Role DependentRole { get; set; }
        public Role DependedOnRole { get; set; }

        public Function DependentFunction { get; set; }
        public Function DependedOnFunction { get; set; }

        public Index DependentIndex { get; set; }
        public Index DependendOnIndex { get; set; }

        public Package DependentPackage { get; set; }
        public Package DependendOnPackage { get; set; }

        protected override Dependency Self => this;

        protected override EquatableProperty<Dependency>[] EquatableProperties => s_EquatableProperties;

        public TypeObjectNameKey TypeObjectNameKey => m_PrimitiveTypeKey ?? (m_PrimitiveTypeKey = new TypeObjectNameKey(GetType(), TypeObjectNameKey.BuildObjectName(SqlHelper, PackageName, DependedOnName, DependentName)));

        internal static Dependency CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values)
        {
            var result =
                new Dependency(sqlHelper)
                {
                    DependentName = new Identifier(sqlHelper, values["RDB$DEPENDENT_NAME"].DbValueToString()),
                    DependedOnName = new Identifier(sqlHelper, values["RDB$DEPENDED_ON_NAME"].DbValueToString()),
                    FieldName = new Identifier(sqlHelper, values["RDB$FIELD_NAME"].DbValueToString()),
                    DependentType = (ObjectType)values["RDB$DEPENDENT_TYPE"].DbValueToInt32().GetValueOrDefault(),
                    DependedOnType = (ObjectType)values["RDB$DEPENDED_ON_TYPE"].DbValueToInt32().GetValueOrDefault()
                };
            result.DependentNameKey = new Identifier(sqlHelper, result.DependentName);
            result.DependedOnNameKey = new Identifier(sqlHelper, result.DependedOnName);

            if (sqlHelper.TargetVersion.AtLeast(TargetVersion.Version30))
            {
                result.PackageName = new Identifier(sqlHelper, values["RDB$PACKAGE_NAME"].DbValueToString());
                result.DependentNameKey = new Identifier(sqlHelper, result.DependentName);
                result.DependedOnNameKey = new Identifier(sqlHelper, result.PackageName, result.DependedOnName);
            }
            return result;
        }
    }
}
