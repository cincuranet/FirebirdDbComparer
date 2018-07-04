using System.Collections.Generic;
using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Interfaces
{
    public interface IMetadataFields : IDatabaseObject
    {
        IDictionary<Identifier, Field> Fields { get; }
        IDictionary<Identifier, Field> Domains { get; }
        IEnumerable<CommandGroup> CreateDomains(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> AlterDomains(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> DropDomains(IMetadata other, IComparerContext context);
    }
}
