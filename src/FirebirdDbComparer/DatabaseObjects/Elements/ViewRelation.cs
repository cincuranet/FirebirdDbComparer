using System.Collections.Generic;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Compare;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Elements
{
    public sealed class ViewRelation : SqlElement<ViewRelation>
    {
        private static readonly EquatableProperty<ViewRelation>[] s_EquatableProperties =
        {
            new EquatableProperty<ViewRelation>(x => x.ViewName, nameof(ViewName)),
            new EquatableProperty<ViewRelation>(x => x.ViewContextName, nameof(ViewContextName)),
            new EquatableProperty<ViewRelation>(x => x.ViewContext, nameof(ViewContext)),
            new EquatableProperty<ViewRelation>(x => x.ContextName, nameof(ContextName)),
            new EquatableProperty<ViewRelation>(x => x.PackageName, nameof(PackageName))
        };

        private ViewRelation(ISqlHelper sqlHelper)
            : base(sqlHelper)
        { }

        public Identifier ViewName { get; private set; }
        public Identifier ViewContextName { get; private set; }
        public int ViewContext { get; private set; }
        public string ContextName { get; private set; }
        public ContextTypeType? ContextType { get; private set; }
        public Identifier PackageName { get; private set; }

        public Relation View { get; set; }
        public Relation ContextView { get; set; }
        public Relation ContextRelation { get; set; }
        public Procedure ContextProcedure { get; set; }
        public Package Package { get; set; }

        protected override ViewRelation Self => this;

        protected override EquatableProperty<ViewRelation>[] EquatableProperties => s_EquatableProperties;

        internal static ViewRelation CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values)
        {
            var result =
                new ViewRelation(sqlHelper)
                {
                    ViewName = new Identifier(sqlHelper, values["RDB$VIEW_NAME"].DbValueToString()),
                    ViewContextName = new Identifier(sqlHelper, values["RDB$RELATION_NAME"].DbValueToString()),
                    ViewContext = values["RDB$VIEW_CONTEXT"].DbValueToInt32().GetValueOrDefault(),
                    ContextName = values["RDB$CONTEXT_NAME"].DbValueToString()
                };

            if (sqlHelper.TargetVersion.AtLeast(TargetVersion.Version30))
            {
                result.ContextType = (ContextTypeType)values["RDB$CONTEXT_TYPE"].DbValueToInt32().GetValueOrDefault();
                result.PackageName = new Identifier(sqlHelper, values["RDB$PACKAGE_NAME"].DbValueToString());
            }
            return result;
        }
    }
}
