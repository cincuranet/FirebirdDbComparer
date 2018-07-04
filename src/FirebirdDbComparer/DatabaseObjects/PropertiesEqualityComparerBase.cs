using System;
using System.Collections.Generic;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;

namespace FirebirdDbComparer.DatabaseObjects
{
    internal abstract class PropertiesEqualityComparerBase<T> : IEqualityComparer<T>
    {
        private readonly EquatableProperty<T>[] m_EquatableProperties;

        public PropertiesEqualityComparerBase(EquatableProperty<T>[] equatableProperties)
        {
            m_EquatableProperties = equatableProperties;
        }

        public bool Equals(T x, T y)
        {
            return EquatableHelper.ElementaryEquals(x, y) ?? EquatableHelper.PropertiesEqual(x, y, m_EquatableProperties);
        }

        public int GetHashCode(T obj)
        {
            return EquatableHelper.GetHashCode(obj, m_EquatableProperties);
        }
    }
}
