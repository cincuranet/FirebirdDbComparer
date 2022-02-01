using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataDatabase25 : DatabaseObject, IMetadataDatabase, ISupportsComment
{
    public MetadataDatabase25(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    public DatabaseStringOrdinal Description { get; private set; }
    public Identifier CharacterSetName { get; private set; }
    public CharacterSet CharacterSet { get; private set; }
    public int Dialect { get; private set; }
    public short OdsMajor { get; private set; }
    public short OdsMinor { get; private set; }

    protected virtual string CommandText => @"
select D.RDB$DESCRIPTION,
       trim(D.RDB$CHARACTER_SET_NAME) as RDB$CHARACTER_SET_NAME,
       M.MON$SQL_DIALECT,
       M.MON$ODS_MAJOR,
       M.MON$ODS_MINOR
  from RDB$DATABASE D
       cross join MON$DATABASE M";

    public override void Initialize()
    {
        var values = Execute(CommandText).Single();
        Initialize(values);
    }

    [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
    protected virtual void Initialize(IDictionary<string, object> values)
    {
        Description = values["RDB$DESCRIPTION"].DbValueToString();
        CharacterSetName = new Identifier(SqlHelper, values["RDB$CHARACTER_SET_NAME"].DbValueToString());
        Dialect = values["MON$SQL_DIALECT"].DbValueToInt32().Value;
        OdsMajor = values["MON$ODS_MAJOR"].DbValueToInt16().Value;
        OdsMinor = values["MON$ODS_MINOR"].DbValueToInt16().Value;
    }

    public override void FinishInitialization()
    {
        CharacterSet =
            Metadata
                .MetadataCharacterSets
                .CharacterSetsByName[CharacterSetName];
    }

    IEnumerable<CommandGroup> ISupportsComment.Handle(IMetadata other, IComparerContext context)
    {
        if (Description == null && other.MetadataDatabase.Description != null)
        {
            yield return new CommandGroup().Append(new Command().Append("COMMENT ON DATABASE IS NULL"));
        }
        else if (Description != null && Description != other.MetadataDatabase.Description)
        {
            yield return new CommandGroup().Append(new Command().Append($"COMMENT ON DATABASE IS '{SqlHelper.DoubleSingleQuotes(Description)}'"));
        }
    }

    public IEnumerable<CommandGroup> HandleDatabase(IMetadata other, IComparerContext context)
    {
        if (Dialect != 3 || other.MetadataDatabase.Dialect != 3)
        {
            throw new NotSupportedException("Only Dialect 3 databases are supported.");
        }
        if (CharacterSet.CharacterSetId != other.MetadataDatabase.CharacterSet.CharacterSetId)
        {
            throw new InvalidOperationException($"Databases have different character sets: {CharacterSet.CharacterSetName} and {other.MetadataDatabase.CharacterSet.CharacterSetName}.");
        }
        yield break;
    }
}
