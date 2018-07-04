using System;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects
{
    public abstract class BasicElement<T> : IElement, IEquatable<T> where T : class
    {
        public override bool Equals(object obj)
        {
            return EquatableHelper.ElementaryEqualsThenEquatableEquals(this, obj);
        }

        public bool Equals(T other)
        {
            return EquatableHelper.ElementaryEquals(this, other) ?? EquatableHelper.PropertiesEqual(this, other, EquatableProperties);
        }

        public override int GetHashCode()
        {
            return EquatableHelper.GetHashCode(this, EquatableProperties);
        }

        public static bool operator ==(BasicElement<T> left, BasicElement<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BasicElement<T> left, BasicElement<T> right)
        {
            return !Equals(left, right);
        }

        public static implicit operator T(BasicElement<T> element)
        {
            return element.Self;
        }

        protected abstract T Self { get; }

        protected abstract EquatableProperty<T>[] EquatableProperties { get; }
    }
}
