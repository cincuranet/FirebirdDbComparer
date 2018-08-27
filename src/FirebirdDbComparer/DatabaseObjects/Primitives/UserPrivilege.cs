using System;
using System.Collections.Generic;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Primitives
{
    public sealed class UserPrivilege : Primitive<UserPrivilege>
    {
        private static readonly EquatableProperty<UserPrivilege>[] s_EquatableProperties =
        {
            new EquatableProperty<UserPrivilege>(x => x.User, nameof(User)),
            new EquatableProperty<UserPrivilege>(x => x.Grantor, nameof(Grantor)),
            new EquatableProperty<UserPrivilege>(x => x.Privilege, nameof(Privilege)),
            new EquatableProperty<UserPrivilege>(x => x.GrantOption, nameof(GrantOption)),
            new EquatableProperty<UserPrivilege>(x => x.ObjectName, nameof(ObjectName)),
            new EquatableProperty<UserPrivilege>(x => x.FieldName, nameof(FieldName)),
            new EquatableProperty<UserPrivilege>(x => x.UserType, nameof(UserType)),
            new EquatableProperty<UserPrivilege>(x => x.ObjectType, nameof(ObjectType))
        };

        public UserPrivilege(ISqlHelper sqlHelper)
            : base(sqlHelper)
        { }

        public Identifier User { get; private set; }
        public Identifier Grantor { get; private set; }
        public Privilege Privilege { get; private set; }
        public bool GrantOption { get; private set; }
        public Identifier ObjectName { get; private set; }
        public Identifier FieldName { get; private set; }
        public ObjectType UserType { get; private set; }
        public ObjectType ObjectType { get; private set; }

        public bool IsSystemGeneratedObject { get; private set; }
        public Identifier LegacyFunctionNameKey { get; private set; }

        public Relation Relation { get; set; }
        public Procedure Procedure { get; set; }
        public DbException DbException { get; set; }
        public Field Field { get; set; }
        public CharacterSet CharacterSet { get; set; }
        public Role Role { get; set; }
        public Generator Generator { get; set; }
        public Function Function { get; set; }
        public Collation Collation { get; set; }
        public Package Package { get; set; }

        protected override UserPrivilege Self => this;

        protected override EquatableProperty<UserPrivilege>[] EquatableProperties => s_EquatableProperties;

        protected override IEnumerable<Command> OnCreate(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            throw new InvalidOperationException();
        }

        protected override IEnumerable<Command> OnDrop(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            throw new InvalidOperationException();
        }

        protected override IEnumerable<Command> OnAlter(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            throw new InvalidOperationException();
        }

        protected override Identifier OnPrimitiveTypeKeyObjectName() => TypeObjectNameKey.BuildObjectName(SqlHelper, LegacyFunctionNameKey ?? ObjectName, FieldName);

        internal static UserPrivilege CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values)
        {
            var result =
                new UserPrivilege(sqlHelper)
                {
                    User = new Identifier(sqlHelper, values["RDB$USER"].DbValueToString()),
                    Grantor = new Identifier(sqlHelper, values["RDB$GRANTOR"].DbValueToString()),
                    Privilege = ConvertFrom(values["RDB$PRIVILEGE"].DbValueToString()),
                    GrantOption = values["RDB$GRANT_OPTION"].DbValueToBool().GetValueOrDefault(),
                    ObjectName = new Identifier(sqlHelper, values["RDB$RELATION_NAME"].DbValueToString()),
                    FieldName = new Identifier(sqlHelper, values["RDB$FIELD_NAME"].DbValueToString()),
                    UserType = (ObjectType)values["RDB$USER_TYPE"].DbValueToInt32().GetValueOrDefault(),
                    ObjectType = (ObjectType)values["RDB$OBJECT_TYPE"].DbValueToInt32().GetValueOrDefault()
                };
            result.IsSystemGeneratedObject = sqlHelper.HasSystemPrefix(result.ObjectName);
            if (result.ObjectType == ObjectType.UDF)
            {
                result.LegacyFunctionNameKey = new Identifier(sqlHelper, $".{result.ObjectName}");
            }
            return result;
        }

        private static Privilege ConvertFrom(string privilege)
        {
            Privilege result;
            switch (privilege)
            {
                case "S":
                    result = Privilege.SELECT;
                    break;
                case "U":
                    result = Privilege.UPDATE;
                    break;
                case "D":
                    result = Privilege.DELETE;
                    break;
                case "I":
                    result = Privilege.INSERT;
                    break;
                case "X":
                    result = Privilege.EXECUTE;
                    break;
                case "R":
                    result = Privilege.REFERENCE;
                    break;
                case "M":
                    result = Privilege.MEMBER;
                    break;
                case "G":
                    result = Privilege.USAGE;
                    break;
                case "C":
                    result = Privilege.CREATE;
                    break;
                case "L":
                    result = Privilege.ALTER;
                    break;
                case "O":
                    result = Privilege.DROP;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown privilege: {privilege}.");
            }
            return result;
        }
    }
}
