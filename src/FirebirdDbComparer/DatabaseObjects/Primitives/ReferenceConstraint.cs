using System;
using System.Collections.Generic;
using System.Diagnostics;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Primitives
{
    [DebuggerDisplay("ConstraintName:{ConstraintName} ConstraintNameUq:{ConstraintNameUq}")]
    public sealed class ReferenceConstraint : Primitive<ReferenceConstraint>
    {
        private static readonly EquatableProperty<ReferenceConstraint>[] s_EquatableProperties =
        {
            new EquatableProperty<ReferenceConstraint>(x => x._EqualityConstraintName, nameof(ConstraintName)),
            new EquatableProperty<ReferenceConstraint>(x => x._EqualityConstraintNameUq, nameof(ConstraintNameUq)),
            new EquatableProperty<ReferenceConstraint>(x => x.UpdateRule, nameof(UpdateRule)),
            new EquatableProperty<ReferenceConstraint>(x => x.DeleteRule, nameof(DeleteRule))
        };

        public ReferenceConstraint(ISqlHelper sqlHelper)
            : base(sqlHelper)
        { }

        internal Identifier _EqualityConstraintName => SqlHelper.IsImplicitIntegrityConstraintName(ConstraintName) ? null : ConstraintName;
        public Identifier ConstraintName { get; private set; }
        internal Identifier _EqualityConstraintNameUq => SqlHelper.IsImplicitIntegrityConstraintName(ConstraintNameUq) ? null : ConstraintNameUq;
        public Identifier ConstraintNameUq { get; private set; }
        public ConstraintRule UpdateRule { get; private set; }
        public ConstraintRule DeleteRule { get; private set; }
        public IList<Trigger> Triggers { get; set; }
        public RelationConstraint RelationConstraint { get; set; }
        public RelationConstraint RelationConstraintUq { get; set; }

        protected override ReferenceConstraint Self => this;

        protected override EquatableProperty<ReferenceConstraint>[] EquatableProperties => s_EquatableProperties;

        protected override IEnumerable<Command> OnCreate(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            throw new InvalidOperationException();
        }

        protected override IEnumerable<Command> OnDrop(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            throw new InvalidOperationException();
        }

        protected override IEnumerable<Command> OnAlter(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            throw new InvalidOperationException();
        }

        protected override Identifier OnPrimitiveTypeKeyObjectName() => ConstraintName;

        internal static ReferenceConstraint CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values)
        {
            var result =
                new ReferenceConstraint(sqlHelper)
                {
                    ConstraintName = new Identifier(sqlHelper, values["RDB$CONSTRAINT_NAME"].DbValueToString()),
                    ConstraintNameUq = new Identifier(sqlHelper, values["RDB$CONST_NAME_UQ"].DbValueToString()),
                    UpdateRule = ConvertFrom(values["RDB$UPDATE_RULE"]),
                    DeleteRule = ConvertFrom(values["RDB$DELETE_RULE"])
                };
            return result;
        }

        private static ConstraintRule ConvertFrom(object constraintRule)
        {
            var rule = constraintRule.DbValueToString();
            switch (rule)
            {
                case "CASCADE":
                    return ConstraintRule.Cascade;
                case "RESTRICT":
                    return ConstraintRule.Restrict;
                case "SET DEFAULT":
                    return ConstraintRule.SetDefault;
                case "SET NULL":
                    return ConstraintRule.SetNull;
                case "NO ACTION":
                    return ConstraintRule.NoAction;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown constraint rule: {rule}.");
            }
        }
    }
}
