using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataExceptions25 : DatabaseObject, IMetadataExceptions, ISupportsComment
{
    private IDictionary<int, DbException> m_ExceptionsById;
    private IDictionary<Identifier, DbException> m_ExceptionsByName;

    public MetadataExceptions25(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    public IDictionary<Identifier, DbException> ExceptionsByName => m_ExceptionsByName;

    public IDictionary<int, DbException> ExceptionsById => m_ExceptionsById;

    protected virtual string CommandText => @"
select trim(E.RDB$EXCEPTION_NAME) as RDB$EXCEPTION_NAME,
       E.RDB$EXCEPTION_NUMBER,
       E.RDB$MESSAGE,
       E.RDB$DESCRIPTION,
       E.RDB$SYSTEM_FLAG
  from RDB$EXCEPTIONS E";

    public override void Initialize()
    {
        var exceptions = Execute(CommandText)
            .Select(o => DbException.CreateFrom(SqlHelper, o))
            .ToArray();
        m_ExceptionsById = exceptions.ToDictionary(x => x.ExceptionNumber);
        m_ExceptionsByName = exceptions.ToDictionary(x => x.ExceptionName);
    }

    public override void FinishInitialization()
    { }

    IEnumerable<CommandGroup> ISupportsComment.Handle(IMetadata other, IComparerContext context)
    {
        var result = new CommandGroup().Append(HandleComment(ExceptionsByName, other.MetadataExceptions.ExceptionsByName, x => x.ExceptionName, "EXCEPTION", x => new[] { x.ExceptionName }, context));
        if (!result.IsEmpty)
        {
            yield return result;
        }
    }

    public IEnumerable<CommandGroup> CreateExceptions(IMetadata other, IComparerContext context)
    {
        return FilterSystemFlagUser(ExceptionsByName.Values)
            .Where(e => !other.MetadataExceptions.ExceptionsByName.ContainsKey(e.ExceptionName))
            .Select(e => new CommandGroup().Append(e.Create(Metadata, other, context)));
    }

    public IEnumerable<CommandGroup> DropExceptions(IMetadata other, IComparerContext context)
    {
        return FilterSystemFlagUser(other.MetadataExceptions.ExceptionsByName.Values)
            .Where(e => !ExceptionsByName.ContainsKey(e.ExceptionName))
            .Select(e => new CommandGroup().Append(e.Drop(Metadata, other, context)));
    }

    public IEnumerable<CommandGroup> AlterExceptions(IMetadata other, IComparerContext context)
    {
        return FilterSystemFlagUser(ExceptionsByName.Values)
            .Where(e => other.MetadataExceptions.ExceptionsByName.TryGetValue(e.ExceptionName, out var otherException) && e != otherException)
            .Select(e => new CommandGroup().Append(e.Alter(Metadata, other, context)))
            .Where(x => !x.IsEmpty);
    }
}
