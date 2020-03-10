using System;
using System.Diagnostics;

using FirebirdDbComparer.Common;

namespace FirebirdDbComparer.DatabaseObjects
{
    [DebuggerDisplay("{ToString()}")]
    public class DatabaseStringOrdinal : IComparable<DatabaseStringOrdinal>, IEquatable<DatabaseStringOrdinal>
    {
        public DatabaseStringOrdinal(string value)
        {
            Value = value;
        }

        public static implicit operator DatabaseStringOrdinal(string value)
        {
            return new DatabaseStringOrdinal(value);
        }

        public string Value { get; }

        public override string ToString() => Value;

        public override int GetHashCode() => Value?.GetHashCode() ?? 0;

        public override bool Equals(object obj) => EquatableHelper.ElementaryEqualsThenEquatableEquals(this, obj);

        public int GetHashCode(DatabaseStringOrdinal obj) => obj.GetHashCode();

        public static bool operator ==(DatabaseStringOrdinal x, DatabaseStringOrdinal y) => CompareImpl(x, y) == 0;

        public static bool operator !=(DatabaseStringOrdinal x, DatabaseStringOrdinal y) => CompareImpl(x, y) != 0;

        public int CompareTo(DatabaseStringOrdinal other) => CompareImpl(this, other);

        public bool Equals(DatabaseStringOrdinal other) => CompareImpl(this, other) == 0;

        private static int CompareImpl(DatabaseStringOrdinal x, DatabaseStringOrdinal y) => string.CompareOrdinal(x?.Value, y?.Value);

        public char this[int index] => Value?[index] ?? throw new IndexOutOfRangeException();
    }
}
