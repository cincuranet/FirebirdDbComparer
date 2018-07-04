using System.Collections.Generic;

using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Interfaces
{
    public interface IComparerContext
    {
        IComparerSettings Settings { get; }
        HashSet<TypeObjectNameKey> DroppedObjects { get; }
        bool IsDropped(TypeObjectNameKey key);
        HashSet<TypeObjectNameKey> CreatedObjects { get; }
        bool IsCreated(TypeObjectNameKey key);
        HashSet<RelationFieldKey> CreatedRelationFields { get; }
        List<CommandGroup> DeferredColumnsToDrop { get; }
        bool EmptyBodiesEnabled { get; }
        void EnableEmptyBodies();
        void DisableEmptyBodies();
    }
}
