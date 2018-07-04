using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.DatabaseObjects.Elements;
using FirebirdDbComparer.SqlGeneration;
using FirebirdDbComparer.DatabaseObjects;

namespace FirebirdDbComparer.Interfaces
{
    public interface IMetadataRelations : IDatabaseObject
    {
        IDictionary<Identifier, Relation> ExternalTables { get; }
        IDictionary<Identifier, Relation> GttTables { get; }
        IDictionary<Identifier, Relation> Relations { get; }
        IDictionary<Identifier, Relation> Tables { get; }
        IDictionary<Identifier, Relation> Views { get; }
        IDictionary<RelationFieldKey, RelationField> RelationFields { get; }
        IDictionary<TypeObjectNameKey, RelationField> RelationFieldByTypeObjectNameKey { get; }
        IDictionary<Field, IList<RelationField>> RelationFieldsByField { get; }
        IList<ViewRelation> ViewRelations { get; }
        IEnumerable<CommandGroup> CreateTablesWithEmpty(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> AlterCreatedOrAlteredTablesToFull(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> AlterTablesAndToEmptyForAlteringOrDropping(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> CreateEmptyViews(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> AlterViewsToFullBody(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> HandleTableFieldsPositions(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> AlterViewsToEmptyBodyForAlteringOrDropping(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> DropViews(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> DropTables(IMetadata other, IComparerContext context);
    }
}
