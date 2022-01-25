using System.Collections.Generic;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Compare;
using FirebirdDbComparer.Exceptions;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Primitives
{
    public sealed class CharacterSet : Primitive<CharacterSet>, IHasSystemFlag, IHasDescription
    {
        private static readonly EquatableProperty<CharacterSet>[] s_EquatableProperties =
        {
            new EquatableProperty<CharacterSet>(x => x.CharacterSetName, nameof(CharacterSetName)),
            new EquatableProperty<CharacterSet>(x => x.NumberOfCharacters, nameof(NumberOfCharacters)),
            new EquatableProperty<CharacterSet>(x => x.DefaultCollateName, nameof(DefaultCollateName)),
            new EquatableProperty<CharacterSet>(x => x.SystemFlag, nameof(SystemFlag)),
            new EquatableProperty<CharacterSet>(x => x.BytesPerCharacter, nameof(BytesPerCharacter)),
            new EquatableProperty<CharacterSet>(x => x.OwnerName, nameof(OwnerName))
        };

        public CharacterSet(ISqlHelper sqlHelper)
            : base(sqlHelper)
        { }

        public Identifier CharacterSetName { get; private set; }
        public int? NumberOfCharacters { get; private set; }
        public Identifier DefaultCollateName { get; private set; }
        public Collation DefaultCollation { get; set; }
        public int CharacterSetId { get; private set; }
        public SystemFlagType SystemFlag { get; private set; }
        public DatabaseStringOrdinal Description { get; private set; }
        public int BytesPerCharacter { get; private set; }
        public Identifier OwnerName { get; private set; }
        public IList<Collation> Collations { get; set; }

        protected override CharacterSet Self => this;

        protected override EquatableProperty<CharacterSet>[] EquatableProperties => s_EquatableProperties;

        protected override IEnumerable<Command> OnCreate(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            throw new NotSupportedOnFirebirdException($"Creating charset is not supported ({CharacterSetName}).");
        }

        protected override IEnumerable<Command> OnDrop(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            throw new NotSupportedOnFirebirdException($"Dropping charset is not supported ({CharacterSetName}).");
        }

        protected override IEnumerable<Command> OnAlter(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            var otherCharacterSet = FindOtherChecked(targetMetadata.MetadataCharacterSets.CharacterSetsByName, CharacterSetName, "character set");

            if (EquatableHelper.PropertiesEqual(this, otherCharacterSet, EquatableProperties, nameof(DefaultCollateName)))
            {
                yield return new Command()
                    .Append($"ALTER CHARACTER SET {CharacterSetName.AsSqlIndentifier()} SET DEFAULT COLLATION {DefaultCollateName.AsSqlIndentifier()}");
            }
            else
            {
                throw new NotSupportedOnFirebirdException($"Altering charset is not supported ({CharacterSetName}).");
            }
        }

        protected override Identifier OnPrimitiveTypeKeyObjectName() => CharacterSetName;

        internal static CharacterSet CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values)
        {
            var result =
                new CharacterSet(sqlHelper)
                {
                    CharacterSetName = new Identifier(sqlHelper, values["RDB$CHARACTER_SET_NAME"].DbValueToString()),
                    NumberOfCharacters = values["RDB$NUMBER_OF_CHARACTERS"].DbValueToInt32(),
                    DefaultCollateName = new Identifier(sqlHelper, values["RDB$DEFAULT_COLLATE_NAME"].DbValueToString()),
                    CharacterSetId = values["RDB$CHARACTER_SET_ID"].DbValueToInt32().GetValueOrDefault(),
                    SystemFlag = (SystemFlagType)values["RDB$SYSTEM_FLAG"].DbValueToInt32().GetValueOrDefault(),
                    Description = values["RDB$DESCRIPTION"].DbValueToString(),
                    BytesPerCharacter = values["RDB$BYTES_PER_CHARACTER"].DbValueToInt32().GetValueOrDefault()
                };

            if (sqlHelper.TargetVersion.AtLeast(TargetVersion.Version30))
            {
                result.OwnerName = new Identifier(sqlHelper, values["RDB$OWNER_NAME"].DbValueToString());
            }
            return result;
        }
    }
}
