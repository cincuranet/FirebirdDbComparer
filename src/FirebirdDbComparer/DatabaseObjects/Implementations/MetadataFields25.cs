using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations
{
    public class MetadataFields25 : DatabaseObject, IMetadataFields, ISupportsComment
    {
        private IDictionary<Identifier, Field> m_Fields;
        private IDictionary<Identifier, Field> m_Domains;

        public MetadataFields25(IMetadata metadata, ISqlHelper sqlHelper)
            : base(metadata, sqlHelper)
        { }

        public IDictionary<Identifier, Field> Domains => m_Domains;

        public IDictionary<Identifier, Field> Fields => m_Fields;

        // RDB$FIELD_SUB_TYPE: weird discrepancy in some databases
        // RDB$FIELD_SCALE: weird discrepancy in some databases
        // RDB$FIELD_PRECISION: CORE-5550
        protected virtual string CommandText => @"
select trim(F.RDB$FIELD_NAME) as RDB$FIELD_NAME,
       trim(F.RDB$DESCRIPTION) as RDB$DESCRIPTION,
       trim(F.RDB$COMPUTED_SOURCE) as RDB$COMPUTED_SOURCE,
       trim(F.RDB$DEFAULT_SOURCE) as RDB$DEFAULT_SOURCE,
       trim(F.RDB$VALIDATION_SOURCE) as RDB$VALIDATION_SOURCE,
       F.RDB$FIELD_TYPE,
       coalesce(F.RDB$FIELD_SUB_TYPE, 0) as RDB$FIELD_SUB_TYPE,
       F.RDB$FIELD_LENGTH,
       coalesce(F.RDB$FIELD_SCALE, 0) as RDB$FIELD_SCALE,
       coalesce(F.RDB$FIELD_PRECISION, 0) as RDB$FIELD_PRECISION,
       F.RDB$SEGMENT_LENGTH,
       F.RDB$CHARACTER_LENGTH,
       F.RDB$CHARACTER_SET_ID,
       F.RDB$COLLATION_ID,
       iif(coalesce(F.RDB$NULL_FLAG, 0) = 0, 0, 1) as RDB$NULL_FLAG,
       F.RDB$SYSTEM_FLAG
  from RDB$FIELDS F";

        public override void Initialize()
        {
            m_Fields = Execute(CommandText)
                .Select(o => Field.CreateFrom(SqlHelper, o))
                .ToDictionary(x => x.FieldName);
            m_Domains = m_Fields.Values
                .Where(x => x.MetadataFieldType == MetadataFieldType.Domain)
                .ToDictionary(x => x.FieldName);
        }

        public override void FinishInitialization()
        {
            foreach (var field in Fields.Values.Where(f => f.CharacterSetId != null))
            {
                var characterSetId = (int)field.CharacterSetId;
                field.CharacterSet = Metadata.MetadataCharacterSets.CharacterSetsById[characterSetId];
                if (field.CollationId != null)
                {
                    var collationId = (int)field.CollationId;
                    field.Collation = Metadata.MetadataCollations.CollationsByKey[new CollationKey(characterSetId, collationId)];
                }
            }
        }

        IEnumerable<CommandGroup> ISupportsComment.Handle(IMetadata other, IComparerContext context)
        {
            var result = new CommandGroup().Append(HandleComment(Domains, other.MetadataFields.Domains, x => x.FieldName, "DOMAIN", x => new[] { x.FieldName }, context));
            if (!result.IsEmpty)
            {
                yield return result;
            }
        }

        public IEnumerable<CommandGroup> CreateDomains(IMetadata other, IComparerContext context)
        {
            return FilterSystemFlagUser(Domains.Values)
                .Where(d => !other.MetadataFields.Domains.ContainsKey(d.FieldName))
                .Select(d => new CommandGroup().Append(d.Create(Metadata, other, context)));
        }

        public IEnumerable<CommandGroup> DropDomains(IMetadata other, IComparerContext context)
        {
            return FilterSystemFlagUser(other.MetadataFields.Domains.Values)
                .Where(d => !Domains.ContainsKey(d.FieldName))
                .Select(d => new CommandGroup().Append(d.Drop(Metadata, other, context)));
        }

        public IEnumerable<CommandGroup> AlterDomains(IMetadata other, IComparerContext context)
        {
            return FilterSystemFlagUser(Domains.Values)
                .Where(d => other.MetadataFields.Domains.TryGetValue(d.FieldName, out var otherDomain) && d != otherDomain)
                .Select(d => new CommandGroup()
                            .Append(Metadata.MetadataConstraints.DropConstraintsForDependenciesHelper(s => s.RelationField.FieldSource == d.FieldName, other, context))
                            .Append(Metadata.MetadataIndices.DropIndicesForDependenciesHelper(s => s.RelationField.FieldSource == d.FieldName, other, context))
                            .Append(d.Alter(Metadata, other, context)))
                .Where(x => !x.IsEmpty);
        }
    }
}
