using System;
using System.Diagnostics;

namespace FirebirdDbComparer.Common.Equatable
{
    [DebuggerDisplay("{FullName}")]
    public sealed class EquatableProperty<T>
    {
        public string FullName => $"{typeof(T).FullName}.{Name}";

        public EquatableProperty(Func<T, object> valueFactory, string name)
        {
            ValueFactory = valueFactory;
            Name = name;
        }

        public Func<T, object> ValueFactory { get; }
        public string Name { get; }
    }
}
