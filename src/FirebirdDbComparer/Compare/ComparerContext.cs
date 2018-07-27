using System;
using System.Collections.Generic;

using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Compare
{
    public sealed class ComparerContext : IComparerContext
    {
        private bool m_EmptyBodiesEnabled;

        public ComparerContext(IComparerSettings settings)
        {
            DroppedObjects = new HashSet<TypeObjectNameKey>();
            CreatedRelationFields = new HashSet<RelationFieldKey>();
            DeferredColumnsToDrop = new List<CommandGroup>();
            m_EmptyBodiesEnabled = false;
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public IComparerSettings Settings { get; }

        public HashSet<TypeObjectNameKey> DroppedObjects { get; }

        public HashSet<RelationFieldKey> CreatedRelationFields { get; }

        public List<CommandGroup> DeferredColumnsToDrop { get; }

        public bool EmptyBodiesEnabled => m_EmptyBodiesEnabled;

        public void EnableEmptyBodies()
        {
            if (m_EmptyBodiesEnabled)
                throw new InvalidOperationException();

            m_EmptyBodiesEnabled = true;
        }

        public void DisableEmptyBodies()
        {
            if (!m_EmptyBodiesEnabled)
                throw new InvalidOperationException();

            m_EmptyBodiesEnabled = false;
        }
    }
}
