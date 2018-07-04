using System;
using System.Collections.Generic;
using System.Linq;
using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.Elements;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Interfaces
{
    public interface IMetadataConstraints : IDatabaseObject
    {
        IList<CheckConstraint> CheckConstraints { get; }
        IDictionary<Identifier, ReferenceConstraint> ReferenceConstraintsByName { get; }
        IDictionary<Identifier, IList<ReferenceConstraint>> ReferenceConstraintsByNameUq { get; }
        IDictionary<Identifier, IList<ReferenceConstraint>> ReferenceConstraintsByRelation { get; }
        IDictionary<Identifier, RelationConstraint> RelationConstraintsByName { get; }
        IDictionary<Identifier, RelationConstraint> RelationConstraintsByIndexName { get; }
        IDictionary<Identifier, IList<RelationConstraint>> RelationConstraintsByRelation { get; }
        IEnumerable<CommandGroup> HandleConstraints(IMetadata other, IComparerContext context);
        IEnumerable<Command> DropConstraintsForDependenciesHelper(Func<IndexSegment, bool> selector, IMetadata other, IComparerContext context);
    }
}
