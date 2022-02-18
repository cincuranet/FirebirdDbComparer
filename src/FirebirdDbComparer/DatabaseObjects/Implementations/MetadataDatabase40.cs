using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataDatabase40 : MetadataDatabase30
{
    public MetadataDatabase40(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    protected override string CommandText => @"
select D.RDB$DESCRIPTION,
       trim(D.RDB$CHARACTER_SET_NAME) as RDB$CHARACTER_SET_NAME,
       M.MON$SQL_DIALECT,
       M.MON$ODS_MAJOR,
       M.MON$ODS_MINOR,
       D.RDB$LINGER,
       D.RDB$SQL_SECURITY
  from RDB$DATABASE D
       cross join MON$DATABASE M";

    public override CommandGroup ProcessDatabase(IMetadata other, IComparerContext context)
    {
        var result = base.ProcessDatabase(other, context) ?? new CommandGroup();

        if (Database.SqlSecurity == null && other.MetadataDatabase.Database.SqlSecurity != null)
        {
#warning HERE
            result.Append(new Command().Append("ALTER DATABASE DROP SQL SECURITY"));
        }
        else if (Database.SqlSecurity != null && other.MetadataDatabase.Database.SqlSecurity != null && Database.SqlSecurity != other.MetadataDatabase.Database.SqlSecurity)
        {
            result.Append(new Command().Append($"ALTER DATABASE SET DEFAULT SQL SECURITY {SqlHelper.SqlSecurityString(Database.SqlSecurity)}"));
        }

        return !result.IsEmpty ? result : null;
    }
}
