using System.Collections.Generic;

using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Interfaces;

public interface IComparerContext
{
    IComparerSettings Settings { get; }
    HashSet<TypeObjectNameKey> DroppedObjects { get; }
    HashSet<RelationFieldKey> CreatedRelationFields { get; }
    List<CommandGroup> DeferredColumnsToDrop { get; }
    bool EmptyBodiesEnabled { get; }
    void EnableEmptyBodies();
    void DisableEmptyBodies();
}
