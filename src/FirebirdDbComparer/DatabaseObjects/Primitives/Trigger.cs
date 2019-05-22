using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Compare;
using FirebirdDbComparer.Exceptions;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Primitives
{
    [DebuggerDisplay("{RelationName}.{TriggerName}")]
    public sealed class Trigger : Primitive<Trigger>, IHasSystemFlag, IHasDescription
    {
        private static readonly EquatableProperty<Trigger>[] s_EquatableProperties =
        {
            new EquatableProperty<Trigger>(x => x.TriggerName, nameof(TriggerName)),
            new EquatableProperty<Trigger>(x => x.RelationName, nameof(RelationName)),
            new EquatableProperty<Trigger>(x => x.TriggerSequence, nameof(TriggerSequence)),
            new EquatableProperty<Trigger>(x => x.TriggerType, nameof(TriggerType)),
            new EquatableProperty<Trigger>(x => x.TriggerSource, nameof(TriggerSource)),
            new EquatableProperty<Trigger>(x => x.Inactive, nameof(Inactive)),
            new EquatableProperty<Trigger>(x => x.SystemFlag, nameof(SystemFlag)),
            new EquatableProperty<Trigger>(x => x.EngineName, nameof(EngineName)),
            new EquatableProperty<Trigger>(x => x.EntryPoint, nameof(EntryPoint))
        };

        public Trigger(ISqlHelper sqlHelper)
            : base(sqlHelper)
        { }

        public Identifier TriggerName { get; private set; }
        public Identifier RelationName { get; private set; }
        public int TriggerSequence { get; private set; }
        internal ulong TriggerType { get; private set; }
        public TriggerClassType TriggerClass { get; private set; }
        public TriggerBeforeAfterType? TriggerBeforeAfter { get; private set; }
        public IList<TriggerEventType> TriggerEvents { get; private set; }
        public DatabaseStringOrdinal TriggerSource { get; private set; }
        public DatabaseStringOrdinal Description { get; private set; }
        public bool Inactive { get; private set; }
        public SystemFlagType SystemFlag { get; private set; }
        public Identifier EngineName { get; private set; }
        public DatabaseStringOrdinal EntryPoint { get; private set; }
        public RelationConstraint Constraint { get; set; }
        public Relation Relation { get; set; }

        protected override Trigger Self => this;

        protected override EquatableProperty<Trigger>[] EquatableProperties => s_EquatableProperties;

        protected override IEnumerable<Command> OnCreate(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            var command = new PSqlCommand();
            command
                .Append($"CREATE OR ALTER TRIGGER {TriggerName.AsSqlIndentifier()}")
                .AppendLine()
                .Append($"{(Inactive ? "INACTIVE" : "ACTIVE")} ");

            switch (TriggerClass)
            {
                case TriggerClassType.DML:
                case TriggerClassType.DDL:
                    command.Append($"{TriggerBeforeAfter.ToDescription()} {TriggerEvents[0].ToDescription()} ");
                    foreach (var item in TriggerEvents.Skip(1))
                    {
                        command.Append($"OR {item.ToDescription()} ");
                    }
                    break;
                case TriggerClassType.DB:
                    command.Append($"ON {TriggerEvents[0].ToDescription()} ");
                    break;
            }

            command
                .Append($"POSITION {TriggerSequence}")
                .AppendLine();

            if (TriggerClass == TriggerClassType.DML)
            {
                command
                    .Append($"ON {RelationName.AsSqlIndentifier()}")
                    .AppendLine();
            }

            command.Append(TriggerSource);
            yield return command;
        }

        protected override IEnumerable<Command> OnDrop(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            yield return new Command()
                .Append($"DROP TRIGGER {TriggerName.AsSqlIndentifier()}");
        }

        protected override IEnumerable<Command> OnAlter(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            var otherTrigger = FindOtherChecked(targetMetadata.MetadataTriggers.TriggersByName, TriggerName, "trigger");
            if ((TriggerClass != otherTrigger.TriggerClass)
                || (TriggerClass == TriggerClassType.DB && TriggerType != otherTrigger.TriggerType)
                || (TriggerClass == TriggerClassType.DDL && TriggerType != otherTrigger.TriggerType))
            {
                throw new NotSupportedOnFirebirdException($"Altering DB and DDL trigger type is not supported ({TriggerName}).");
            }
            else
            {
                return OnCreate(sourceMetadata, targetMetadata, context);
            }
        }

        protected override Identifier OnPrimitiveTypeKeyObjectName() => TriggerName;

        internal static Trigger CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values)
        {
            var type = values["RDB$TRIGGER_TYPE"].DbValueToUInt64().GetValueOrDefault();
            var (@class, beforeAfter, events) = TriggerTypeParser.Parse(type);

            var result =
                new Trigger(sqlHelper)
                {
                    TriggerName = new Identifier(sqlHelper, values["RDB$TRIGGER_NAME"].DbValueToString()),
                    RelationName = new Identifier(sqlHelper, values["RDB$RELATION_NAME"].DbValueToString()),
                    TriggerSequence = values["RDB$TRIGGER_SEQUENCE"].DbValueToInt32().GetValueOrDefault(),
                    TriggerType = type,
                    TriggerClass = @class,
                    TriggerBeforeAfter = beforeAfter,
                    TriggerEvents = events,
                    TriggerSource = values["RDB$TRIGGER_SOURCE"].DbValueToString(),
                    Description = values["RDB$DESCRIPTION"].DbValueToString(),
                    Inactive = values["RDB$TRIGGER_INACTIVE"].DbValueToBool().GetValueOrDefault(),
                    SystemFlag = (SystemFlagType)values["RDB$SYSTEM_FLAG"].DbValueToInt32().GetValueOrDefault()
                };

            if (sqlHelper.TargetVersion.AtLeast30())
            {
                result.EngineName = new Identifier(sqlHelper, values["RDB$ENGINE_NAME"].DbValueToString());
                result.EntryPoint = values["RDB$ENTRYPOINT"].DbValueToString();
            }

            return result;
        }        
    }
}
