using System;
using System.Collections.Generic;
using System.Linq;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataDatabase25 : DatabaseObject, IMetadataDatabase, ISupportsComment
{
    private Database m_Database;

    public MetadataDatabase25(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    public Database Database => m_Database;

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
        m_Database = Execute(CommandText)
            .Select(d => Database.CreateFrom(SqlHelper, d))
            .Single();
    }

    public override void FinishInitialization()
    {
        Database.CharacterSet =
            Metadata
                .MetadataCharacterSets
                .CharacterSetsByName[Database.CharacterSetName];
    }

    IEnumerable<CommandGroup> ISupportsComment.Handle(IMetadata other, IComparerContext context)
    {
        var result = new CommandGroup();

        if (Database.Description == null && other.MetadataDatabase.Database.Description != null)
        {
            result.Append(new Command().Append("COMMENT ON DATABASE IS NULL"));
        }
        else if (Database.Description != null && Database.Description != other.MetadataDatabase.Database.Description)
        {
            result.Append(new Command().Append($"COMMENT ON DATABASE IS '{SqlHelper.DoubleSingleQuotes(Database.Description)}'"));
        }

        if (!result.IsEmpty)
        {
            yield return result;
        }
    }

    public virtual CommandGroup ProcessDatabase(IMetadata other, IComparerContext context)
    {
        if (Database.Dialect != 3 || other.MetadataDatabase.Database.Dialect != 3)
        {
            throw new NotSupportedException("Only Dialect 3 databases are supported.");
        }
        if (Database.CharacterSet.CharacterSetId != other.MetadataDatabase.Database.CharacterSet.CharacterSetId)
        {
            throw new InvalidOperationException($"Databases have different character sets: {Database.CharacterSet.CharacterSetName} and {other.MetadataDatabase.Database.CharacterSet.CharacterSetName}.");
        }
        return null;
    }
}
