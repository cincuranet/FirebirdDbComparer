using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            yield return "USER_MANAGEMENT";
        if (m_Value[2])
            yield return "READ_RAW_PAGES";
        if (m_Value[3])
            yield return "CREATE_USER_TYPES";
        if (m_Value[4])
            yield return "USE_NBACKUP_UTILITY";
        if (m_Value[5])
            yield return "CHANGE_SHUTDOWN_MODE";
        if (m_Value[6])
            yield return "TRACE_ANY_ATTACHMENT";
        if (m_Value[7])
            yield return "MONITOR_ANY_ATTACHMENT";
        if (m_Value[8])
            yield return "ACCESS_SHUTDOWN_DATABASE";
        if (m_Value[9])
            yield return "CREATE_DATABASE";
        if (m_Value[10])
            yield return "DROP_DATABASE";
        if (m_Value[11])
            yield return "USE_GBAK_UTILITY";
        if (m_Value[12])
            yield return "USE_GSTAT_UTILITY";
        if (m_Value[13])
            yield return "USE_GFIX_UTILITY";
        if (m_Value[14])
            yield return "IGNORE_DB_TRIGGERS";
        if (m_Value[15])
            yield return "CHANGE_HEADER_SETTINGS";
        if (m_Value[16])
            yield return "SELECT_ANY_OBJECT_IN_DATABASE";
        if (m_Value[17])
            yield return "ACCESS_ANY_OBJECT_IN_DATABASE";
        if (m_Value[18])
            yield return "MODIFY_ANY_OBJECT_IN_DATABASE";
        if (m_Value[19])
            yield return "CHANGE_MAPPING_RULES";
        if (m_Value[20])
            yield return "USE_GRANTED_BY_CLAUSE";
        if (m_Value[21])
            yield return "GRANT_REVOKE_ON_ANY_OBJECT";
        if (m_Value[22])
            yield return "GRANT_REVOKE_ANY_DDL_RIGHT";
        if (m_Value[23])
            yield return "CREATE_PRIVILEGED_ROLES";
        if (m_Value[24])
            yield return "GET_DBCRYPT_INFO";
        if (m_Value[25])
            yield return "MODIFY_EXT_CONN_POOL";
        if (m_Value[26])
            yield return "REPLICATE_INTO_DATABASE";
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
