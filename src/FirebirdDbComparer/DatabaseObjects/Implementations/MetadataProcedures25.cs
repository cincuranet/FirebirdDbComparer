using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.DatabaseObjects.Elements;
using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations
{
    public class MetadataProcedures25 : DatabaseObject, IMetadataProcedures, ISupportsComment
    {
        private IDictionary<ProcedureParameterKey, ProcedureParameter> m_ProcedureParameters;
        private IDictionary<int, Procedure> m_ProceduresById;
        private IDictionary<Identifier, Procedure> m_ProceduresByName;

        public MetadataProcedures25(IMetadata metadata, ISqlHelper sqlHelper)
            : base(metadata, sqlHelper)
        { }

        public IDictionary<ProcedureParameterKey, ProcedureParameter> ProcedureParameters => m_ProcedureParameters;

        public IDictionary<int, Procedure> ProceduresById => m_ProceduresById;

        public virtual IDictionary<Identifier, Procedure> ProceduresByName => m_ProceduresByName;

        protected virtual string ProcedureCommandText => @"
select trim(P.RDB$PROCEDURE_NAME) as RDB$PROCEDURE_NAME,
       P.RDB$PROCEDURE_ID,
       P.RDB$PROCEDURE_INPUTS,
       P.RDB$PROCEDURE_OUTPUTS,
       P.RDB$DESCRIPTION,
       P.RDB$PROCEDURE_SOURCE,
       trim(P.RDB$OWNER_NAME) as RDB$OWNER_NAME,
       P.RDB$PROCEDURE_TYPE,
       P.RDB$SYSTEM_FLAG
  from RDB$PROCEDURES P";

        protected virtual string ProcedureParameterCommandText => @"
select trim(PP.RDB$PARAMETER_NAME) as RDB$PARAMETER_NAME,
       trim(PP.RDB$PROCEDURE_NAME) as RDB$PROCEDURE_NAME,
       PP.RDB$PARAMETER_NUMBER,
       PP.RDB$PARAMETER_TYPE,
       trim(PP.RDB$FIELD_SOURCE) as RDB$FIELD_SOURCE,
       PP.RDB$DESCRIPTION,
       PP.RDB$SYSTEM_FLAG,
       PP.RDB$DEFAULT_SOURCE,
       PP.RDB$COLLATION_ID,
       iif(coalesce(PP.RDB$NULL_FLAG, 0) = 0, 0, 1) as RDB$NULL_FLAG,
       PP.RDB$PARAMETER_MECHANISM,
       trim(PP.RDB$FIELD_NAME) as RDB$FIELD_NAME,
       trim(PP.RDB$RELATION_NAME) as RDB$RELATION_NAME
  from RDB$PROCEDURE_PARAMETERS PP";

        public override void Initialize()
        {
            m_ProcedureParameters =
                Execute(ProcedureParameterCommandText)
                    .Select(o => ProcedureParameter.CreateFrom(SqlHelper, o))
                    .ToDictionary(x => new ProcedureParameterKey(x.ProcedureName, x.ParameterName));
            var procedureParameters = m_ProcedureParameters.Values.ToLookup(x => x.ProcedureName);
            var procedures =
                Execute(ProcedureCommandText)
                    .Select(o => Procedure.CreateFrom(SqlHelper, o, procedureParameters))
                    .ToArray();
            m_ProceduresById = procedures.ToDictionary(x => x.ProcedureId);
            m_ProceduresByName = procedures.ToDictionary(x => x.ProcedureName);
        }

        public override void FinishInitialization()
        {
            foreach (var procedureParameter in ProcedureParameters.Values)
            {
                procedureParameter.Procedure = ProceduresByName[procedureParameter.ProcedureName];
                if (procedureParameter.FieldSource != null)
                {
                    procedureParameter.Field =
                        Metadata
                            .MetadataFields
                            .Fields[procedureParameter.FieldSource];
                    if (procedureParameter.CollationId != null && procedureParameter.Field.CharacterSetId != null)
                    {
                        procedureParameter.Collation =
                            Metadata
                                .MetadataCollations
                                .CollationsByKey[new CollationKey((int)procedureParameter.Field.CharacterSetId, (int)procedureParameter.CollationId)];
                    }
                }
                if (procedureParameter.FieldName != null && procedureParameter.RelationName != null)
                {
                    procedureParameter.RelationField =
                        Metadata
                            .MetadataRelations
                            .RelationFields[new RelationFieldKey(procedureParameter.RelationName, procedureParameter.FieldName)];
                    procedureParameter.Relation =
                        Metadata
                            .MetadataRelations
                            .Relations[procedureParameter.RelationName];
                }
            }
        }

        IEnumerable<CommandGroup> ISupportsComment.Handle(IMetadata other, IComparerContext context)
        {
            var result = new CommandGroup().Append(HandleComment(ProceduresByName, other.MetadataProcedures.ProceduresByName, x => x.ProcedureName, "PROCEDURE", x => new[] { x.ProcedureName }, context, x => HandleCommentNested(x.ProcedureParameters.OrderBy(y => y.ParameterNumber), other.MetadataProcedures.ProcedureParameters, (a, b) => new ProcedureParameterKey(a, b), x.ProcedureName, y => y.ParameterName, "PARAMETER", y => new[] { y.ParameterName }, context)));
            if (!result.IsEmpty)
            {
                yield return result;
            }
        }

        public IEnumerable<CommandGroup> CreateEmptyNewProcedures(IMetadata other, IComparerContext context)
        {
            return FilterNewProcedures(other)
                .Select(procedure => new CommandGroup().Append(WrapActionWithEmptyBody(procedure.Create)(Metadata, other, context)));
        }

        public IEnumerable<CommandGroup> AlterProceduresToFullBody(IMetadata other, IComparerContext context)
        {
            return FilterNewProcedures(other).Concat(FilterProceduresToBeAltered(other))
                .Select(procedure => new CommandGroup().Append(procedure.Alter(Metadata, other, context)))
                .Where(x => !x.IsEmpty);
        }

        public IEnumerable<CommandGroup> AlterProceduresToEmptyBodyForAlteringOrDropping(IMetadata other, IComparerContext context)
        {
            return FilterProceduresToBeDropped(other).Concat(FilterProceduresToBeAltered(other))
                .Select(procedure => new CommandGroup().Append(WrapActionWithEmptyBody(procedure.Alter)(Metadata, other, context)))
                .Where(x => !x.IsEmpty);
        }

        public IEnumerable<CommandGroup> DropProcedures(IMetadata other, IComparerContext context)
        {
            return FilterProceduresToBeDropped(other)
                .Select(procedure => new CommandGroup().Append(procedure.Drop(Metadata, other, context)));
        }

        protected virtual IEnumerable<Procedure> FilterNewProcedures(IMetadata other)
        {
            return FilterSystemFlagUser(ProceduresByName.Values)
                .Where(p => !other.MetadataProcedures.ProceduresByName.ContainsKey(p.ProcedureName));
        }

        protected virtual IEnumerable<Procedure> FilterProceduresToBeDropped(IMetadata other)
        {
            return FilterSystemFlagUser(other.MetadataProcedures.ProceduresByName.Values)
                .Where(p => !ProceduresByName.ContainsKey(p.ProcedureName));
        }

        protected virtual IEnumerable<Procedure> FilterProceduresToBeAltered(IMetadata other)
        {
            return FilterSystemFlagUser(ProceduresByName.Values)
                .Where(p => other.MetadataProcedures.ProceduresByName.TryGetValue(p.ProcedureName, out var otherProcedure) && otherProcedure != p);
        }
    }
}
