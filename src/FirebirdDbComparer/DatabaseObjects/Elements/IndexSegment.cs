using System.Collections.Generic;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Elements
{
    public sealed class IndexSegment : SqlElement<IndexSegment>
    {
        private static readonly EquatableProperty<IndexSegment>[] s_EquatableProperties =
        {
            new EquatableProperty<IndexSegment>(x => x.IndexName, nameof(IndexName)),
            new EquatableProperty<IndexSegment>(x => x.FieldPosition, nameof(FieldPosition)),
            new EquatableProperty<IndexSegment>(x => x.RelationField, nameof(RelationField))
        };

        public IndexSegment(ISqlHelper sqlHelper)
            : base(sqlHelper)
        { }

        public Identifier IndexName { get; private set; }
        public Identifier FieldName { get; private set; }
        public int? FieldPosition { get; private set; }
        public RelationField RelationField { get; set; }

        protected override IndexSegment Self => this;

        protected override EquatableProperty<IndexSegment>[] EquatableProperties => s_EquatableProperties;

        public static IndexSegment CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values)
        {
            var indexSegment =
                new IndexSegment(sqlHelper)
                {
                    IndexName = new Identifier(sqlHelper, values["RDB$INDEX_NAME"].DbValueToString()),
                    FieldName = new Identifier(sqlHelper, values["RDB$FIELD_NAME"].DbValueToString()),
                    FieldPosition = values["RDB$FIELD_POSITION"].DbValueToInt32()
                };
            return indexSegment;
        }
    }
}
