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

        protected override Identifier OnPrimitiveTypeKeyObjectName() => TypeObjectNameKey.BuildObjectName(SqlHelper, ObjectName, FieldName);

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
                    UserType = new ObjectType(sqlHelper, values["RDB$USER_TYPE"].DbValueToInt32().GetValueOrDefault()),
                    ObjectType = new ObjectType(sqlHelper, values["RDB$OBJECT_TYPE"].DbValueToInt32().GetValueOrDefault())
                };
            result.IsSystemGeneratedObject = sqlHelper.HasSystemPrefix(result.ObjectName);
            return result;
        }

        private static Privilege ConvertFrom(string privilege)
        {
            Privilege result;
            switch (privilege)
            {
                case "S":
                    result = Privilege.Select;
                    break;
                case "U":
                    result = Privilege.Update;
                    break;
                case "D":
                    result = Privilege.Delete;
                    break;
                case "I":
                    result = Privilege.Insert;
                    break;
                case "X":
                    result = Privilege.Execute;
                    break;
                case "R":
                    result = Privilege.Reference;
                    break;
                case "M":
                    result = Privilege.Member;
                    break;
                case "G":
                    result = Privilege.Usage;
                    break;
                case "C":
                    result = Privilege.Create;
                    break;
                case "L":
                    result = Privilege.Alter;
                    break;
                case "O":
                    result = Privilege.Drop;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown privilege: {privilege}.");
            }
            return result;
        }
    }
}
