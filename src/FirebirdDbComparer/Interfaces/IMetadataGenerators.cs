using System.Collections.Generic;
using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Interfaces
{
    public interface IMetadataGenerators : IDatabaseObject
    {
        IDictionary<int, Generator> GeneratorsById { get; }
        IDictionary<Identifier, Generator> GeneratorsByName { get; }
        IEnumerable<CommandGroup> CreateGenerators(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> AlterGenerators(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> DropGenerators(IMetadata other, IComparerContext context);
    }
}
