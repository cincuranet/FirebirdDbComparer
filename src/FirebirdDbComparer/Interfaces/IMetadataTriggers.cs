using System.Collections.Generic;
using System.Linq;
using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Interfaces;

public interface IMetadataTriggers : IDatabaseObject
{
    IDictionary<Identifier, Trigger> TriggersByName { get; }
    IDictionary<Identifier, IList<Trigger>> TriggersByRelation { get; }
    IEnumerable<CommandGroup> HandleTriggers(IMetadata other, IComparerContext context);
}
