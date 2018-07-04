using System;
using System.Collections.Generic;
using System.Diagnostics;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Compare;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Primitives
{
    [DebuggerDisplay("{RelationName}.{TriggerName}")]
    public sealed class Trigger : Primitive<Trigger>, IHasSystemFlag, IHasDescription
    {
        private const int TRIGGER_TYPE_SHIFT = 13;
        private const int TRIGGER_TYPE_MASK = 0x3 << TRIGGER_TYPE_SHIFT;

        private static readonly EquatableProperty<Trigger>[] s_EquatableProperties =
        {
            new EquatableProperty<Trigger>(x => x.TriggerName, nameof(TriggerName)),
            new EquatableProperty<Trigger>(x => x.RelationName, nameof(RelationName)),
            new EquatableProperty<Trigger>(x => x.TriggerSequence, nameof(TriggerSequence)),
            new EquatableProperty<Trigger>(x => x.TriggerClass, nameof(TriggerClass)),
            new EquatableProperty<Trigger>(x => x.TriggerAction1, nameof(TriggerAction1)),
            new EquatableProperty<Trigger>(x => x.TriggerAction2, nameof(TriggerAction2)),
            new EquatableProperty<Trigger>(x => x.TriggerAction3, nameof(TriggerAction3)),
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
        public TriggerClassType TriggerClass { get; private set; }
        public TriggerActionType TriggerAction1 { get; private set; }
        public TriggerActionType? TriggerAction2 { get; private set; }
        public TriggerActionType? TriggerAction3 { get; private set; }
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

            if (TriggerClass == TriggerClassType.DB)
            {
                command.Append($"ON {TriggerAction1.ToDescription()} ");
            }
            else if (TriggerClass == TriggerClassType.DML)
            {
                var beforeOrAfter = TriggerAction1.In(new[] { TriggerActionType.BEFORE_INSERT, TriggerActionType.BEFORE_UPDATE, TriggerActionType.BEFORE_DELETE }) ? "BEFORE" : "AFTER";
                command.Append($"{beforeOrAfter} {TriggerAction1.ToDescription()} ");
                if (TriggerAction2 != null)
                {
                    command.Append($"OR {TriggerAction2.ToDescription()} ");
                }
                if (TriggerAction3 != null)
                {
                    command.Append($"OR {TriggerAction3.ToDescription()} ");
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Unknown trigger class {TriggerClass}.");
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
            return OnCreate(sourceMetadata, targetMetadata, context);
        }

        protected override Identifier OnPrimitiveTypeKeyObjectName() => TriggerName;

        internal static Trigger CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values)
        {
            var triggerType = values["RDB$TRIGGER_TYPE"].DbValueToInt32().GetValueOrDefault();
            var triggerActions = GetTriggerActionTypes(triggerType);

            var result =
                new Trigger(sqlHelper)
                {
                    TriggerName = new Identifier(sqlHelper, values["RDB$TRIGGER_NAME"].DbValueToString()),
                    RelationName = new Identifier(sqlHelper, values["RDB$RELATION_NAME"].DbValueToString()),
                    TriggerSequence = values["RDB$TRIGGER_SEQUENCE"].DbValueToInt32().GetValueOrDefault(),
                    TriggerClass = GetTriggerClassType(triggerType),
                    TriggerAction1 = triggerActions[0],
                    TriggerAction2 = triggerActions.Length > 1 ? triggerActions[1] : (TriggerActionType?)null,
                    TriggerAction3 = triggerActions.Length > 2 ? triggerActions[2] : (TriggerActionType?)null,
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

        private static TriggerClassType GetTriggerClassType(int type)
        {
            switch (type & TRIGGER_TYPE_MASK)
            {
                case 0 << TRIGGER_TYPE_SHIFT:
                    return TriggerClassType.DML;
                case 1 << TRIGGER_TYPE_SHIFT:
                    return TriggerClassType.DB;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown trigger class type {type}.");
            }
        }

        private static TriggerActionType[] GetTriggerActionTypes(int type)
        {
            switch (type)
            {
                case 1:
                    return new[] { TriggerActionType.BEFORE_INSERT };
                case 3:
                    return new[] { TriggerActionType.BEFORE_UPDATE };
                case 5:
                    return new[] { TriggerActionType.BEFORE_DELETE };
                case 11:
                    return new[] { TriggerActionType.BEFORE_UPDATE, TriggerActionType.BEFORE_INSERT };
                case 13:
                    return new[] { TriggerActionType.BEFORE_DELETE, TriggerActionType.BEFORE_INSERT };
                case 17:
                    return new[] { TriggerActionType.BEFORE_INSERT, TriggerActionType.BEFORE_UPDATE };
                case 21:
                    return new[] { TriggerActionType.BEFORE_DELETE, TriggerActionType.BEFORE_UPDATE };
                case 25:
                    return new[] { TriggerActionType.BEFORE_INSERT, TriggerActionType.BEFORE_DELETE };
                case 27:
                    return new[] { TriggerActionType.BEFORE_UPDATE, TriggerActionType.BEFORE_DELETE };
                case 53:
                    return new[] { TriggerActionType.BEFORE_DELETE, TriggerActionType.BEFORE_UPDATE, TriggerActionType.BEFORE_INSERT };
                case 59:
                    return new[] { TriggerActionType.BEFORE_UPDATE, TriggerActionType.BEFORE_DELETE, TriggerActionType.BEFORE_INSERT };
                case 77:
                    return new[] { TriggerActionType.BEFORE_DELETE, TriggerActionType.BEFORE_INSERT, TriggerActionType.BEFORE_UPDATE };
                case 89:
                    return new[] { TriggerActionType.BEFORE_INSERT, TriggerActionType.BEFORE_DELETE, TriggerActionType.BEFORE_UPDATE };
                case 107:
                    return new[] { TriggerActionType.BEFORE_UPDATE, TriggerActionType.BEFORE_INSERT, TriggerActionType.BEFORE_DELETE };
                case 113:
                    return new[] { TriggerActionType.BEFORE_INSERT, TriggerActionType.BEFORE_UPDATE, TriggerActionType.BEFORE_DELETE };

                case 2:
                    return new[] { TriggerActionType.AFTER_INSERT };
                case 4:
                    return new[] { TriggerActionType.AFTER_UPDATE };
                case 6:
                    return new[] { TriggerActionType.AFTER_DELETE };
                case 12:
                    return new[] { TriggerActionType.AFTER_UPDATE, TriggerActionType.AFTER_INSERT };
                case 14:
                    return new[] { TriggerActionType.AFTER_DELETE, TriggerActionType.AFTER_INSERT };
                case 18:
                    return new[] { TriggerActionType.AFTER_INSERT, TriggerActionType.AFTER_UPDATE };
                case 22:
                    return new[] { TriggerActionType.AFTER_DELETE, TriggerActionType.AFTER_UPDATE };
                case 26:
                    return new[] { TriggerActionType.AFTER_INSERT, TriggerActionType.AFTER_DELETE };
                case 28:
                    return new[] { TriggerActionType.AFTER_UPDATE, TriggerActionType.AFTER_DELETE };
                case 54:
                    return new[] { TriggerActionType.AFTER_DELETE, TriggerActionType.AFTER_UPDATE, TriggerActionType.AFTER_INSERT };
                case 60:
                    return new[] { TriggerActionType.AFTER_UPDATE, TriggerActionType.AFTER_DELETE, TriggerActionType.AFTER_INSERT };
                case 78:
                    return new[] { TriggerActionType.AFTER_DELETE, TriggerActionType.AFTER_INSERT, TriggerActionType.AFTER_UPDATE };
                case 90:
                    return new[] { TriggerActionType.AFTER_INSERT, TriggerActionType.AFTER_DELETE, TriggerActionType.AFTER_UPDATE };
                case 108:
                    return new[] { TriggerActionType.AFTER_UPDATE, TriggerActionType.AFTER_INSERT, TriggerActionType.AFTER_DELETE };
                case 114:
                    return new[] { TriggerActionType.AFTER_INSERT, TriggerActionType.AFTER_UPDATE, TriggerActionType.AFTER_DELETE };

                case 8192:
                    return new[] { TriggerActionType.CONNECT };
                case 8193:
                    return new[] { TriggerActionType.DISCONNECT };
                case 8194:
                    return new[] { TriggerActionType.TRANS_START };
                case 8195:
                    return new[] { TriggerActionType.TRANS_COMMIT };
                case 8196:
                    return new[] { TriggerActionType.TRANS_ROLLBACK };

                default:
                    throw new ArgumentOutOfRangeException($"Unknown trigger action type {type}.");
            }
        }
    }
}
