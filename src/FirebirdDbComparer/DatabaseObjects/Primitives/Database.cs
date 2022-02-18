using System;
using System.Collections.Generic;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Compare;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Primitives;

public sealed class Database : Primitive<Database>, IHasDescription
{
    public Database(ISqlHelper sqlHelper)
        : base(sqlHelper)
    { }

    public DatabaseStringOrdinal Description { get; private set; }
    public Identifier CharacterSetName { get; private set; }
    public int Dialect { get; private set; }
    public short OdsMajor { get; private set; }
    public short OdsMinor { get; private set; }
    public int? Linger { get; private set; }
    public bool? SqlSecurity { get; private set; }
    public CharacterSet CharacterSet { get; set; }

    protected override Database Self => this;

    protected override EquatableProperty<Database>[] EquatableProperties => throw new InvalidOperationException();

    protected override IEnumerable<Command> OnCreate(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
    {
        throw new InvalidOperationException();
    }

    protected override IEnumerable<Command> OnDrop(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
    {
        throw new InvalidOperationException();
    }

    protected override IEnumerable<Command> OnAlter(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
    {
        return OnCreate(sourceMetadata, targetMetadata, context);
    }

    protected override Identifier OnPrimitiveTypeKeyObjectName() => throw new InvalidOperationException();

    internal static Database CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values)
    {
        var result =
            new Database(sqlHelper)
            {
                Description = values["RDB$DESCRIPTION"].DbValueToString(),
                CharacterSetName = new Identifier(sqlHelper, values["RDB$CHARACTER_SET_NAME"].DbValueToString()),
                Dialect = values["MON$SQL_DIALECT"].DbValueToInt32().Value,
                OdsMajor = values["MON$ODS_MAJOR"].DbValueToInt16().Value,
                OdsMinor = values["MON$ODS_MINOR"].DbValueToInt16().Value
            };

        if (sqlHelper.TargetVersion.AtLeast(TargetVersion.Version30))
        {
            result.Linger = values["RDB$LINGER"].DbValueToInt32();
        }

        if (sqlHelper.TargetVersion.AtLeast(TargetVersion.Version40))
        {
            result.SqlSecurity = values["RDB$SQL_SECURITY"].DbValueToBool();
        }

        return result;
    }
}
