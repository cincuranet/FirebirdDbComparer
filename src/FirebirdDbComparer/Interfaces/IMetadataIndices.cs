using System;
using System.Collections.Generic;
using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.Elements;
using FirebirdDbComparer.SqlGeneration;
using Index = FirebirdDbComparer.DatabaseObjects.Primitives.Index;

namespace FirebirdDbComparer.Interfaces
{
    public interface IMetadataIndices : IDatabaseObject
    {
        IDictionary<Identifier, Index> Indices { get; }
        IDictionary<Identifier, IList<Index>> IndicesByRelation { get; }
        IEnumerable<CommandGroup> DropIndices(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> CreateAlterRecreateIndices(IMetadata other, IComparerContext context);
        IEnumerable<Command> DropIndicesForDependenciesHelper(Func<IndexSegment, bool> selector, IMetadata other, IComparerContext context);
    }
}
