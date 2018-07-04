using System.Collections.Generic;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Compare;
using FirebirdDbComparer.Exceptions;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Primitives
{
    public sealed class Collation : Primitive<Collation>, IHasSystemFlag, IHasDescription
    {
        private static readonly EquatableProperty<Collation>[] s_EquatableProperties =
        {
            new EquatableProperty<Collation>(x => x.CollationName, nameof(CollationName)),
            new EquatableProperty<Collation>(x => x.CharacterSetId, nameof(CharacterSetId)),
            new EquatableProperty<Collation>(x => x.CollationAttributes, nameof(CollationAttributes)),
            new EquatableProperty<Collation>(x => x.SystemFlag, nameof(SystemFlag)),
            new EquatableProperty<Collation>(x => x.FunctionName, nameof(FunctionName)),
            new EquatableProperty<Collation>(x => x.BaseCollationName, nameof(BaseCollationName)),
            new EquatableProperty<Collation>(x => x.SpecificAttributes, nameof(SpecificAttributes)),
            new EquatableProperty<Collation>(x => x.OwnerName, nameof(OwnerName))
        };

        public Collation(ISqlHelper sqlHelper)
            : base(sqlHelper)
        { }

        public Identifier CollationName { get; private set; }
        public int CollationId { get; private set; }
        public int CharacterSetId { get; private set; }
        public CharacterSet CharacterSet { get; set; }
        public CollationAttributes? CollationAttributes { get; private set; }
        public SystemFlagType SystemFlag { get; private set; }
        public DatabaseStringOrdinal Description { get; private set; }
        public DatabaseStringOrdinal FunctionName { get; set; }
        public Identifier BaseCollationName { get; private set; }
        public DatabaseStringOrdinal SpecificAttributes { get; private set; }
        public Identifier OwnerName { get; private set; }

        protected override Collation Self => this;

        protected override EquatableProperty<Collation>[] EquatableProperties => s_EquatableProperties;

        protected override IEnumerable<Command> OnCreate(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            var command = new Command();
            command.Append($"CREATE COLLATION {CollationName.AsSqlIndentifier()}");
            command.AppendLine();
            command.Append($"FOR {CharacterSet.CharacterSetName.AsSqlIndentifier()}");
            if (FunctionName != null)
            {
                command.AppendLine();
                command.Append($"FROM EXTERNAL ('{SqlHelper.DoubleSingleQuotes(FunctionName)}')");
            }
            else if (BaseCollationName != null)
            {
                command.AppendLine();
                command.Append($"FROM {BaseCollationName.AsSqlIndentifier()}");
            }
            command.AppendLine();
            if (CollationAttributes != null)
            {
                var collationAttributes = (CollationAttributes)CollationAttributes;
                command.Append(collationAttributes.HasFlag(DatabaseObjects.CollationAttributes.TEXTTYPE_ATTR_PAD_SPACE)
                                   ? "PAD SPACE"
                                   : "NO PAD");
                command.AppendLine();
                command.Append(collationAttributes.HasFlag(DatabaseObjects.CollationAttributes.TEXTTYPE_ATTR_CASE_INSENSITIVE)
                                   ? "CASE INSENSITIVE"
                                   : "CASE SENSITIVE");
                command.AppendLine();
                command.Append(collationAttributes.HasFlag(DatabaseObjects.CollationAttributes.TEXTTYPE_ATTR_ACCENT_INSENSITIVE)
                                   ? "ACCENT INSENSITIVE"
                                   : "ACCENT SENSITIVE");
                command.AppendLine();
            }
            if (SpecificAttributes != null)
            {
                command.Append($"'{SqlHelper.DoubleSingleQuotes(SpecificAttributes)}'");
            }
            yield return command;
        }

        protected override IEnumerable<Command> OnDrop(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            yield return new Command()
                .Append($"DROP COLLATION {CollationName}");
        }

        protected override IEnumerable<Command> OnAlter(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            throw new NotSupportedOnFirebirdException($"Altering collation is not supported ({CollationName}).");
        }

        protected override Identifier OnPrimitiveTypeKeyObjectName() => CollationName;

        internal static Collation CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values)
        {
            var result =
                new Collation(sqlHelper)
                {
                    CollationName = new Identifier(sqlHelper, values["RDB$COLLATION_NAME"].DbValueToString()),
                    CollationId = values["RDB$COLLATION_ID"].DbValueToInt32().GetValueOrDefault(),
                    CharacterSetId = values["RDB$CHARACTER_SET_ID"].DbValueToInt32().GetValueOrDefault(),
                    CollationAttributes = (CollationAttributes?)values["RDB$COLLATION_ATTRIBUTES"].DbValueToInt32(),
                    SystemFlag = (SystemFlagType)values["RDB$SYSTEM_FLAG"].DbValueToInt32().GetValueOrDefault(),
                    Description = values["RDB$DESCRIPTION"].DbValueToString(),
                    FunctionName = values["RDB$FUNCTION_NAME"].DbValueToString(),
                    BaseCollationName = new Identifier(sqlHelper, values["RDB$BASE_COLLATION_NAME"].DbValueToString()),
                    SpecificAttributes = values["RDB$SPECIFIC_ATTRIBUTES"].DbValueToString()
                };

            if (sqlHelper.TargetVersion.AtLeast30())
            {
                result.OwnerName = new Identifier(sqlHelper, values["RDB$OWNER_NAME"].DbValueToString());
            }
            return result;
        }
    }
}
