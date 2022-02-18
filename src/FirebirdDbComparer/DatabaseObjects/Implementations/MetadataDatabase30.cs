using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataDatabase30 : MetadataDatabase25
{
    public MetadataDatabase30(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    protected override string CommandText => @"
select D.RDB$DESCRIPTION,
       trim(D.RDB$CHARACTER_SET_NAME) as RDB$CHARACTER_SET_NAME,
       M.MON$SQL_DIALECT,
       M.MON$ODS_MAJOR,
       M.MON$ODS_MINOR,
       D.RDB$LINGER
  from RDB$DATABASE D
       cross join MON$DATABASE M";

    public override CommandGroup ProcessDatabase(IMetadata other, IComparerContext context)
    {
        var result = base.ProcessDatabase(other, context) ?? new CommandGroup();

        var linger = Database.Linger ?? 0;
        var otherLinger = other.MetadataDatabase.Database.Linger ?? 0;
        if (linger != otherLinger)
        {
            result.Append(new Command().Append($"ALTER DATABASE SET LINGER TO {linger}"));
        }

        return !result.IsEmpty ? result : null;
    }
}
