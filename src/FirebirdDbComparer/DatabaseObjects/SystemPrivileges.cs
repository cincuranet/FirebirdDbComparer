using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using FirebirdDbComparer.Common;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects;

[DebuggerDisplay("{ToString()}")]
public class SystemPrivileges : IEquatable<SystemPrivileges>
{
    private readonly BitArray m_Value;

    public ISqlHelper SqlHelper { get; }

    public SystemPrivileges(ISqlHelper sqlHelper, byte[] value)
    {
        m_Value = new BitArray(value);
        SqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof(sqlHelper));
    }

    public IEnumerable<string> ToPrivileges()
    {
        if (m_Value[1])
            yield return SqlHelper.SystemPrivilegeString(1);
        if (m_Value[2])
            yield return SqlHelper.SystemPrivilegeString(2);
        if (m_Value[3])
            yield return SqlHelper.SystemPrivilegeString(3);
        if (m_Value[4])
            yield return SqlHelper.SystemPrivilegeString(4);
        if (m_Value[5])
            yield return SqlHelper.SystemPrivilegeString(5);
        if (m_Value[6])
            yield return SqlHelper.SystemPrivilegeString(6);
        if (m_Value[7])
            yield return SqlHelper.SystemPrivilegeString(7);
        if (m_Value[8])
            yield return SqlHelper.SystemPrivilegeString(8);
        if (m_Value[9])
            yield return SqlHelper.SystemPrivilegeString(9);
        if (m_Value[10])
            yield return SqlHelper.SystemPrivilegeString(10);
        if (m_Value[11])
            yield return SqlHelper.SystemPrivilegeString(11);
        if (m_Value[12])
            yield return SqlHelper.SystemPrivilegeString(12);
        if (m_Value[13])
            yield return SqlHelper.SystemPrivilegeString(13);
        if (m_Value[14])
            yield return SqlHelper.SystemPrivilegeString(14);
        if (m_Value[15])
            yield return SqlHelper.SystemPrivilegeString(15);
        if (m_Value[16])
            yield return SqlHelper.SystemPrivilegeString(16);
        if (m_Value[17])
            yield return SqlHelper.SystemPrivilegeString(17);
        if (m_Value[18])
            yield return SqlHelper.SystemPrivilegeString(18);
        if (m_Value[19])
            yield return SqlHelper.SystemPrivilegeString(19);
        if (m_Value[20])
            yield return SqlHelper.SystemPrivilegeString(20);
        if (m_Value[21])
            yield return SqlHelper.SystemPrivilegeString(21);
        if (m_Value[22])
            yield return SqlHelper.SystemPrivilegeString(22);
        if (m_Value[23])
            yield return SqlHelper.SystemPrivilegeString(23);
        if (m_Value[24])
            yield return SqlHelper.SystemPrivilegeString(24);
        if (m_Value[25])
            yield return SqlHelper.SystemPrivilegeString(25);
        if (m_Value[26])
            yield return SqlHelper.SystemPrivilegeString(26);
    }

    public override int GetHashCode() => m_Value?.GetHashCode() ?? 0;

    public override string ToString() => m_Value?.ToString();

    public override bool Equals(object obj) => EquatableHelper.ElementaryEqualsThenEquatableEquals(this, obj);

    public bool Equals(SystemPrivileges other) => this == other;

    public static bool operator ==(SystemPrivileges x, SystemPrivileges y) => EquatableHelper.ElementaryEquals(x, y) ?? EquatableHelper.ElementaryEquals(x.m_Value, y.m_Value) ?? BitArrayEquals(x.m_Value, y.m_Value);

    public static bool operator !=(SystemPrivileges x, SystemPrivileges y) => !(x == y);

    private static bool BitArrayEquals(BitArray x, BitArray y)
    {
        if (x.Length != y.Length)
            return false;
        for (var i = 0; i < x.Length; i++)
        {
            if (x[i] != y[i])
                return false;
        }
        return true;
    }
}
