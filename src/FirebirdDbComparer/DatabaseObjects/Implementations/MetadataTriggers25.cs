using System;
using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations
{
    public class MetadataTriggers25 : DatabaseObject, IMetadataTriggers, ISupportsComment
    {
        private IDictionary<Identifier, Trigger> m_TriggersByName;
        private IDictionary<Identifier, IList<Trigger>> m_TriggersByRelation;

        public MetadataTriggers25(IMetadata metadata, ISqlHelper sqlHelper)
            : base(metadata, sqlHelper)
        { }

        public IDictionary<Identifier, Trigger> TriggersByName => m_TriggersByName;

        public IDictionary<Identifier, IList<Trigger>> TriggersByRelation => m_TriggersByRelation;

        protected virtual string CommandText => @"
select trim(T.RDB$TRIGGER_NAME) as RDB$TRIGGER_NAME,
       trim(T.RDB$RELATION_NAME) as RDB$RELATION_NAME,
       T.RDB$TRIGGER_SEQUENCE,
       T.RDB$TRIGGER_TYPE,
       T.RDB$TRIGGER_SOURCE,
       T.RDB$DESCRIPTION,
       T.RDB$TRIGGER_INACTIVE,
       T.RDB$SYSTEM_FLAG
  from RDB$TRIGGERS T";

        public override void Initialize()
        {
            m_TriggersByName = Execute(CommandText)
                .Select(o => Trigger.CreateFrom(SqlHelper, o))
                .ToDictionary(x => x.TriggerName);
            m_TriggersByRelation = m_TriggersByName.Values
                .Where(x => x.TriggerClass == TriggerClassType.DML)
                .ToMultiDictionary(x => x.RelationName);
        }

        public override void FinishInitialization()
        {
            var checkConstraintsByTriggerName = Metadata.MetadataConstraints.CheckConstraints.ToLookup(c => c.TriggerName);

            foreach (var trigger in TriggersByName.Values)
            {
                if (trigger.SystemFlag == SystemFlagType.CHECK_CONSTRAINT)
                {
                    trigger.Constraint =
                        checkConstraintsByTriggerName[trigger.TriggerName]
                            .Select(c => Metadata.MetadataConstraints.RelationConstraintsByName[c.ConstraintName])
                            .Single();
                }

                if (trigger.RelationName != null)
                {
                    trigger.Relation =
                        Metadata
                            .MetadataRelations
                            .Relations[trigger.RelationName];
                }
            }
        }

        IEnumerable<CommandGroup> ISupportsComment.Handle(IMetadata other, IComparerContext context)
        {
            var result = new CommandGroup().Append(HandleComment(TriggersByName, other.MetadataTriggers.TriggersByName, x => x.TriggerName, "TRIGGER", x => new[] { x.TriggerName }, context));
            if (!result.IsEmpty)
            {
                yield return result;
            }
        }

        public IEnumerable<CommandGroup> HandleTriggers(IMetadata other, IComparerContext context)
        {
            var result = new CommandGroup()
                .Append(DropTriggersNotExistingInSource(other, context))
                .Append(CreateOrAlterDmlTriggers(other, context))
                .Append(CreateOrAlterDbTriggers(other, context));
            if (!result.IsEmpty)
            {
                yield return result;
            }
        }

        private IEnumerable<Command> CreateOrAlterDbTriggers(IMetadata other, IComparerContext context)
        {
            return CreateOrAlterHelper(t => t.TriggerClass == TriggerClassType.DB, other, context);
        }

        private IEnumerable<Command> CreateOrAlterDmlTriggers(IMetadata other, IComparerContext context)
        {
            return CreateOrAlterHelper(t => t.TriggerClass == TriggerClassType.DML, other, context);
        }

        // even triggers on tables to be dropped need to be explicitly dropped because of possible cyclic dependencies between triggers
        private IEnumerable<Command> DropTriggersNotExistingInSource(IMetadata other, IComparerContext context)
        {
            var triggers = FilterSystemFlagUser(other.MetadataTriggers.TriggersByName.Values)
                .Where(t => !TriggersByName.ContainsKey(t.TriggerName))
                .OrderBy(t => t.RelationName)
                .ThenBy(t => t.TriggerName);
            return triggers.SelectMany(t => t.Drop(Metadata, other, context));
        }

        private IEnumerable<Command> CreateOrAlterHelper(Func<Trigger, bool> predicate, IMetadata other, IComparerContext context)
        {
            var triggers = FilterSystemFlagUser(TriggersByName.Values)
                .Where(predicate)
                .Select(t =>
                        {
                            other.MetadataTriggers.TriggersByName.TryGetValue(t.TriggerName, out var otherTrigger);
                            return new
                                   {
                                       Trigger = t,
                                       Action = otherTrigger == null
                                                    ? t.Create
                                                    : t != otherTrigger
                                                        ? t.Alter
                                                        : (Func<IMetadata, IMetadata, IComparerContext, IEnumerable<Command>>)null
                                   };
                        })
                .Where(x => x.Action != null)
                .OrderBy(x => x.Trigger.RelationName)
                .ThenBy(x => x.Trigger.TriggerName);
            return triggers.SelectMany(x => x.Action(Metadata, other, context));
        }
    }
}
