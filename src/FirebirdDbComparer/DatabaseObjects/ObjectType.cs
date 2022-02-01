using System;
using System.ComponentModel;
using System.Diagnostics;
using FirebirdDbComparer.Common;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects;

[DebuggerDisplay("{ToString()}")]
public class ObjectType : IEquatable<ObjectType>, IComparable<ObjectType>
{
    private readonly int m_Value;

    public ISqlHelper SqlHelper { get; }

    public ObjectType(ISqlHelper sqlHelper, int value)
    {
        m_Value = value;
        SqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof(sqlHelper));
    }

    public string ToSqlObject()
    {
        return SqlHelper.ObjectTypeString(this);
    }

    public bool IsRelation => SqlHelper.ObjectTypeIsRelation(this);
    public bool IsView => SqlHelper.ObjectTypeIsView(this);
    public bool IsTrigger => SqlHelper.ObjectTypeIsTrigger(this);
    public bool IsField => SqlHelper.ObjectTypeIsField(this);
    public bool IsComputedField => SqlHelper.ObjectTypeIsComputedField(this);
    public bool IsProcedure => SqlHelper.ObjectTypeIsProcedure(this);
    public bool IsException => SqlHelper.ObjectTypeIsException(this);
    public bool IsRole => SqlHelper.ObjectTypeIsRole(this);
    public bool IsUser => SqlHelper.ObjectTypeIsUser(this);
    public bool IsUDF => SqlHelper.ObjectTypeIsUDF(this);
    public bool IsExpressionIndex => SqlHelper.ObjectTypeIsExpressionIndex(this);
    public bool IsPackageBody => SqlHelper.ObjectTypeIsPackageBody(this);
    public bool IsPackage => SqlHelper.ObjectTypeIsPackage(this);
    public bool IsCharacterSet => SqlHelper.ObjectTypeIsCharacterSet(this);
    public bool IsGenerator => SqlHelper.ObjectTypeIsGenerator(this);
    public bool IsCollation => SqlHelper.ObjectTypeIsCollation(this);

    public static implicit operator int(ObjectType value)
    {
        return value.m_Value;
    }

    public override int GetHashCode() => m_Value.GetHashCode();

    public override string ToString() => m_Value.ToString();

    public override bool Equals(object obj) => EquatableHelper.ElementaryEqualsThenEquatableEquals(this, obj);

    public bool Equals(ObjectType other) => EquatableHelper.ElementaryEquals(this, other) ?? CompareImpl(this, other) == 0;

    public int CompareTo(ObjectType other) => CompareImpl(this, other);

    public static bool operator ==(ObjectType x, ObjectType y) => CompareImpl(x, y) == 0;

    public static bool operator !=(ObjectType x, ObjectType y) => CompareImpl(x, y) != 0;

    private static int CompareImpl(ObjectType x, ObjectType y) => ReferenceEquals(x, null) ? 1 : ReferenceEquals(y, null) ? -1 : x.m_Value.CompareTo(y.m_Value);
}
