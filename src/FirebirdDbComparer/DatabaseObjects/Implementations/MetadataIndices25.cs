using System;
using System.Collections.Generic;
using System.Linq;
using FirebirdDbComparer.Common;
using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.DatabaseObjects.Elements;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;
using Index = FirebirdDbComparer.DatabaseObjects.Primitives.Index;

namespace FirebirdDbComparer.DatabaseObjects.Implementations
{
    public class MetadataIndices25 : DatabaseObject, IMetadataIndices, ISupportsComment
    {
        private IDictionary<Identifier, Index> m_Indices;
        private IDictionary<Identifier, IList<Index>> m_IndicesByRelation;

        public MetadataIndices25(IMetadata metadata, ISqlHelper sqlHelper)
            : base(metadata, sqlHelper)
        { }

        public IDictionary<Identifier, Index> Indices => m_Indices;

        public IDictionary<Identifier, IList<Index>> IndicesByRelation => m_IndicesByRelation;

        protected virtual string IndexSegmentCommandText => @"
select trim(I.RDB$INDEX_NAME) as RDB$INDEX_NAME,
       trim(I.RDB$FIELD_NAME) as RDB$FIELD_NAME,
       I.RDB$FIELD_POSITION
  from RDB$INDEX_SEGMENTS I";

        protected virtual string IndexCommandText => @"
select trim(I.RDB$INDEX_NAME) as RDB$INDEX_NAME,
       trim(I.RDB$RELATION_NAME) as RDB$RELATION_NAME,
       I.RDB$UNIQUE_FLAG,
       I.RDB$DESCRIPTION,
       I.RDB$SEGMENT_COUNT,
       I.RDB$INDEX_INACTIVE,
       I.RDB$INDEX_TYPE,
       trim(I.RDB$FOREIGN_KEY) as RDB$FOREIGN_KEY,
       I.RDB$EXPRESSION_SOURCE,
       I.RDB$SYSTEM_FLAG
  from RDB$INDICES I";

        public override void Initialize()
        {
            var indexSegments =
                Execute(IndexSegmentCommandText)
                .Select(o => IndexSegment.CreateFrom(SqlHelper, o))
                .ToLookup(x => x.IndexName);
            m_Indices =
                Execute(IndexCommandText)
                .Select(o => Index.CreateFrom(SqlHelper, o, indexSegments))
                .ToDictionary(x => x.IndexName);
            m_IndicesByRelation = m_Indices.Values
                .ToMultiDictionary(x => x.RelationName);
        }

        public override void FinishInitialization()
        {
            foreach (var index in Indices.Values)
            {
                if (Metadata.MetadataConstraints.RelationConstraintsByIndexName.TryGetValue(index.IndexName, out var relationConstraint))
                {
                    index.RelationConstraint = relationConstraint;
                }
                index.Relation = Metadata.MetadataRelations.Relations[index.RelationName];

                foreach (var segment in index.Segments)
                {
                    segment.RelationField =
                        Metadata
                            .MetadataRelations
                            .RelationFields[new RelationFieldKey(index.RelationName, segment.FieldName)];
                }
            }
        }

        IEnumerable<CommandGroup> ISupportsComment.Handle(IMetadata other, IComparerContext context)
        {
            var result = new CommandGroup().Append(HandleComment(Indices, other.MetadataIndices.Indices, x => x.IndexName, "INDEX", x => new[] { x.IndexName }, context, primitivesFilter: x => x.IsUserCreatedIndex));
            if (!result.IsEmpty)
            {
                yield return result;
            }
        }

        public IEnumerable<CommandGroup> DropIndices(IMetadata other, IComparerContext context)
        {
            var result = new CommandGroup();
            result.Append(SelectIndicesHelper(IndicesToBeDroppedPredicate, other.MetadataIndices, Metadata, context).SelectMany(i => i.Drop(Metadata, other, context)));
            if (!result.IsEmpty)
            {
                yield return result;
            }
        }

        public IEnumerable<CommandGroup> CreateAlterRecreateIndices(IMetadata other, IComparerContext context)
        {
            var result = new CommandGroup();
            result.Append(SelectIndicesHelper(IndicesToBeCreatedPredicate, this, other, context).SelectMany(i => i.Create(Metadata, other, context)));
            result.Append(SelectIndicesHelper(IndicesToBeAlteredPredicate, this, other, context).SelectMany(i => i.Alter(Metadata, other, context)));
            result.Append(SelectIndicesHelper(IndicesToBeDropCreatedPredicate, this, other, context).SelectMany(i => DropCreateHelper(i, other, context)));
            result.Append(SelectIndicesHelper(IndicesToBeRecreatedPredicate, this, other, context).SelectMany(i => i.Create(Metadata, other, context)));
            if (!result.IsEmpty)
            {
                yield return result;
            }
        }

        public IEnumerable<Command> DropIndicesForDependenciesHelper(Func<IndexSegment, bool> predicate, IMetadata other, IComparerContext context)
        {
            return other.MetadataIndices.Indices.Values
                .Where(i => i.IsUserCreatedIndex && i.Segments.Any(predicate))
                .Where(i => !context.DroppedObjects.Contains(i.TypeObjectNameKey))
                .SelectMany(i => i.Drop(Metadata, other, context));
        }

        private IEnumerable<Command> DropCreateHelper(Index index, IMetadata other, IComparerContext context)
        {
            if (!context.DroppedObjects.Contains(index.TypeObjectNameKey))
            {
                foreach (var item in index.Drop(Metadata, other, context))
                {
                    yield return item;
                }
            }
            foreach (var item in index.Create(Metadata, other, context))
            {
                yield return item;
            }
        }

        private static IEnumerable<Index> SelectIndicesHelper(Func<Index, IMetadata, IComparerContext, bool> predicate, IMetadataIndices indices, IMetadata other, IComparerContext context)
        {
            return indices.Indices.Values
                .Where(i => i.IsUserCreatedIndex)
                .Where(i => predicate(i, other, context))
                .OrderBy(i => i.RelationName)
                .ThenBy(i => i.IndexName);
        }

        private static bool IndicesToBeCreatedPredicate(Index index, IMetadata metadata, IComparerContext context)
        {
            return !(metadata.MetadataIndices.Indices.TryGetValue(index.IndexName, out var otherIndex) && otherIndex.IsUserCreatedIndex);
        }

        private static bool IndicesToBeAlteredPredicate(Index index, IMetadata metadata, IComparerContext context)
        {
            return metadata.MetadataIndices.Indices.TryGetValue(index.IndexName, out var otherIndex)
                && index != otherIndex
                && index.CanAlter(otherIndex);
        }

        private static bool IndicesToBeDropCreatedPredicate(Index index, IMetadata metadata, IComparerContext context)
        {
            return metadata.MetadataIndices.Indices.TryGetValue(index.IndexName, out var otherIndex)
                && index != otherIndex
                && !index.CanAlter(otherIndex);
        }

        private static bool IndicesToBeRecreatedPredicate(Index index, IMetadata metadata, IComparerContext context)
        {
            return context.DroppedObjects.Contains(index.TypeObjectNameKey)
                && metadata.MetadataIndices.Indices.TryGetValue(index.IndexName, out var otherIndex)
                && index == otherIndex;
        }

        private static bool IndicesToBeDroppedPredicate(Index index, IMetadata metadata, IComparerContext context)
        {
            return !context.DroppedObjects.Contains(index.TypeObjectNameKey)
                && !(metadata.MetadataIndices.Indices.TryGetValue(index.IndexName, out var otherIndex) && otherIndex.IsUserCreatedIndex);
        }
    }
}
