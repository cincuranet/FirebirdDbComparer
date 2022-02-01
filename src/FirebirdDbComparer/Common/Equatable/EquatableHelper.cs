using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FirebirdDbComparer.Common.Equatable;

namespace FirebirdDbComparer.Common;

internal static class EquatableHelper
{
    public static bool? ElementaryEquals<T>(T obj, T other)
    {
        if (ReferenceEquals(obj, other))
        {
            return true;
        }

#pragma warning disable IDE0041 // Use 'is null' check
        if (ReferenceEquals(obj, null))
#pragma warning restore IDE0041 // Use 'is null' check
        {
            return false;
        }

#pragma warning disable IDE0041 // Use 'is null' check
        if (ReferenceEquals(null, other))
#pragma warning restore IDE0041 // Use 'is null' check
        {
            return false;
        }

        return null;
    }

    public static bool ElementaryEqualsThenEquatableEquals<T>(IEquatable<T> obj, object other)
    {
        return ElementaryEquals(obj, other)
            ?? (obj.GetType() != other.GetType()
                ? false
                : obj.Equals((T)other));
    }

    public static bool PropertiesEqual<T>(T obj, T other, params EquatableProperty<T>[] properties)
    {
        return PropertiesEqual(obj, other, properties.AsEnumerable());
    }
    public static bool PropertiesEqual<T>(T obj, T other, EquatableProperty<T>[] properties, params string[] ignoredProperties)
    {
        return PropertiesEqual(obj, other, properties.Where(x => !x.Name.In(ignoredProperties, StringComparer.Ordinal)));
    }
    private static bool PropertiesEqual<T>(T obj, T other, IEnumerable<EquatableProperty<T>> properties)
    {
        return properties.All(
            property =>
            {
                var currentThisProperty = property.ValueFactory(obj);
                var currentOtherProperty = property.ValueFactory(other);
                if (currentThisProperty != null)
                {
                    if (currentThisProperty is IEnumerable)
                    {
                        return Extensions.SetEquals((dynamic)currentThisProperty, (dynamic)currentOtherProperty);
                    }

                    return currentThisProperty.Equals(currentOtherProperty);
                }

                return currentThisProperty == currentOtherProperty;
            });
    }

    public static int GetHashCode<T>(T @this, params EquatableProperty<T>[] properties)
    {
        return
            properties
                .Select(x => x.ValueFactory(@this))
                .Where(x => x != null)
                .Select(x => x.GetHashCode())
                .Aggregate(0, (a, x) => unchecked((a * 397) ^ x));
    }
}
