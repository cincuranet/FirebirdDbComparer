using System;
using System.Diagnostics;

using FirebirdDbComparer.Common;

namespace FirebirdDbComparer.DatabaseObjects
{
    [DebuggerDisplay("{ToString()}")]
    public class DatabaseStringOrdinal : IComparable<DatabaseStringOrdinal>, IEquatable<DatabaseStringOrdinal>
    {
        protected readonly string m_Value;

        public DatabaseStringOrdinal(string value)
        {
            m_Value = value;
        }

        public static implicit operator DatabaseStringOrdinal(string value)
        {
            return new DatabaseStringOrdinal(value);
        }

        public override string ToString() => m_Value;

        public override int GetHashCode() => m_Value?.GetHashCode() ?? 0;

        public override bool Equals(object obj) => EquatableHelper.ElementaryEqualsThenEquatableEquals(this, obj);

        public static bool operator ==(DatabaseStringOrdinal x, DatabaseStringOrdinal y) => CompareImpl(x, y) == 0;

        public static bool operator !=(DatabaseStringOrdinal x, DatabaseStringOrdinal y) => CompareImpl(x, y) != 0;

        public int CompareTo(DatabaseStringOrdinal other) => CompareImpl(this, other);

        public bool Equals(DatabaseStringOrdinal other) => EquatableHelper.ElementaryEquals(this, other) ?? CompareImpl(this, other) == 0;

        private static int CompareImpl(DatabaseStringOrdinal x, DatabaseStringOrdinal y) => string.CompareOrdinal(x?.m_Value, y?.m_Value);

        public char this[int index] => m_Value?[index] ?? throw new IndexOutOfRangeException();
    }
}
