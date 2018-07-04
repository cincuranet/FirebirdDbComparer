using System.Collections.Generic;
using System.Diagnostics;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Elements
{
    [DebuggerDisplay("ConstraintName:{ConstraintName} TriggerName:{TriggerName}")]
    public sealed class CheckConstraint : SqlElement<CheckConstraint>
    {
        private static readonly EquatableProperty<CheckConstraint>[] s_EquatableProperties =
        {
            new EquatableProperty<CheckConstraint>(x => x.ConstraintName, nameof(ConstraintName)),
            new EquatableProperty<CheckConstraint>(x => x.TriggerName, nameof(TriggerName))
        };

        public CheckConstraint(ISqlHelper sqlHelper)
            : base(sqlHelper)
        { }

        public Identifier ConstraintName { get; private set; }
        public Identifier TriggerName { get; private set; }

        protected override CheckConstraint Self => this;

        protected override EquatableProperty<CheckConstraint>[] EquatableProperties => s_EquatableProperties;

        internal static CheckConstraint CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values)
        {
            var result =
                new CheckConstraint(sqlHelper)
                {
                    ConstraintName = new Identifier(sqlHelper, values["RDB$CONSTRAINT_NAME"].DbValueToString()),
                    TriggerName = new Identifier(sqlHelper, values["RDB$TRIGGER_NAME"].DbValueToString())
                };

            return result;
        }
    }
}
