using System;
using System.Diagnostics;

using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.EquatableKeys
{
    [DebuggerDisplay("{Type}.{ObjectName}")]
    public sealed class TypeObjectNameKey : BasicElement<TypeObjectNameKey>
    {
        public static readonly EquatableProperty<TypeObjectNameKey>[] s_EquatableProperties =
        {
            new EquatableProperty<TypeObjectNameKey>(x => x.Type, nameof(Type)),
            new EquatableProperty<TypeObjectNameKey>(x => x.ObjectName, nameof(ObjectName))
        };

        public TypeObjectNameKey(Type type, Identifier objectName)
        {
            if (!typeof(IElement).IsAssignableFrom(type))
            {
                throw new ArgumentException(nameof(type));
            }

            Type = type;
            ObjectName = objectName ?? throw new ArgumentNullException(nameof(objectName));
        }

        public Type Type { get; }
        public Identifier ObjectName { get; }

        protected override TypeObjectNameKey Self => this;

        protected override EquatableProperty<TypeObjectNameKey>[] EquatableProperties => s_EquatableProperties;

        public static Identifier BuildObjectName(ISqlHelper sqlHelper, params object[] items) => new Identifier(sqlHelper, string.Join("::", items));
    }
}
