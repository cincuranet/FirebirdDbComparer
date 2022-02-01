using System.Collections.Generic;
using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Interfaces;

public interface IMetadataExceptions : IDatabaseObject
{
    IDictionary<int, DbException> ExceptionsById { get; }
    IDictionary<Identifier, DbException> ExceptionsByName { get; }
    IEnumerable<CommandGroup> CreateExceptions(IMetadata other, IComparerContext context);
    IEnumerable<CommandGroup> DropExceptions(IMetadata other, IComparerContext context);
    IEnumerable<CommandGroup> AlterExceptions(IMetadata other, IComparerContext context);
}
