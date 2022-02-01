using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Common;

internal static class Extensions
{
    private static T DbValueToHelper<T>(object value, T nullValue, Func<object, T> valueFactory)
    {
        return
            value == null || value == DBNull.Value
                ? nullValue
                : valueFactory(value);
    }

    public static string DbValueToString(this object value)
    {
        return DbValueToHelper(value, null, x => Convert.ToString(x, CultureInfo.InvariantCulture));
    }

    public static short? DbValueToInt16(this object value)
    {
        return DbValueToHelper(value, (short?)null, x => Convert.ToInt16(x, CultureInfo.InvariantCulture));
    }

    public static int? DbValueToInt32(this object value)
    {
        return DbValueToHelper(value, (int?)null, x => Convert.ToInt32(x, CultureInfo.InvariantCulture));
    }

    public static long? DbValueToInt64(this object value)
    {
        return DbValueToHelper(value, (long?)null, x => Convert.ToInt64(x, CultureInfo.InvariantCulture));
    }

    public static ulong? DbValueToUInt64(this object value)
    {
        return DbValueToHelper(value, (ulong?)null, x => Convert.ToUInt64(x, CultureInfo.InvariantCulture));
    }

    public static bool? DbValueToBool(this object value)
    {
        return DbValueToHelper(value, (bool?)null, x => Convert.ToBoolean(x, CultureInfo.InvariantCulture));
    }

    public static bool DbValueToFlag(this object value)
    {
        return DbValueToHelper(value, false, x => Convert.ToInt32(x, CultureInfo.InvariantCulture) != 0);
    }

    public static bool DbValueToNullableFlag(this object value)
    {
        return DbValueToHelper(value, true, x => Convert.ToInt32(x, CultureInfo.InvariantCulture) == 0);
    }

    public static bool SetEquals<T>(this IEnumerable<T> source, IEnumerable<T> other, IEqualityComparer<T> comparer = null)
    {
        return new HashSet<T>(source, comparer).SetEquals(other);
    }

    public static string ToDescription(this Enum value)
    {
        var memberInfo = value.GetType().GetMember(value.ToString());
        var attribute = (DescriptionAttribute)memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault();
        return attribute == null ? value.ToString().ToUpperInvariant() : attribute.Description;
    }

    public static HashSet<T> AddRange<T>(this HashSet<T> source, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            source.Add(item);
        }
        return source;
    }

    public static IDictionary<TKey, IList<TValue>> ToMultiDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
    {
        return source.GroupBy(keySelector).ToDictionary(x => x.Key, x => (IList<TValue>)x.ToList());
    }

    public static IDictionary<TKey, IList<TElement>> ToMultiDictionary<TKey, TValue, TElement>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector, Func<TValue, TElement> elementSelector)
    {
        return source.GroupBy(keySelector).ToDictionary(x => x.Key, x => (IList<TElement>)x.Select(elementSelector).ToList());
    }

    public static bool In<T>(this T value, IEnumerable<T> source, IEqualityComparer<T> comparer = null)
    {
        return source.Contains(value, comparer);
    }

    public static void Push<T>(this IList<T> source, T item)
    {
        source.Insert(0, item);
    }
}
