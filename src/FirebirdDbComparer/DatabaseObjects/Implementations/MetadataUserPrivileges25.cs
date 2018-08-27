using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations
{
    public class MetadataUserPrivileges25 : DatabaseObject, IMetadataUserPrivileges
    {
        private IList<UserPrivilege> m_UserPrivileges;

        public MetadataUserPrivileges25(IMetadata metadata, ISqlHelper sqlHelper)
            : base(metadata, sqlHelper)
        { }

        public IList<UserPrivilege> UserPrivileges => m_UserPrivileges;

        protected virtual string CommandText => @"
select trim(UP.RDB$USER) as RDB$USER,
       trim(UP.RDB$GRANTOR) as RDB$GRANTOR,
       trim(UP.RDB$PRIVILEGE) as RDB$PRIVILEGE,
       UP.RDB$GRANT_OPTION,
       trim(UP.RDB$RELATION_NAME) as RDB$RELATION_NAME,
       trim(UP.RDB$FIELD_NAME) as RDB$FIELD_NAME,
       UP.RDB$USER_TYPE,
       UP.RDB$OBJECT_TYPE
  from RDB$USER_PRIVILEGES UP";

        public override void Initialize()
        {
            m_UserPrivileges =
                Execute(CommandText)
                    .Select(o => UserPrivilege.CreateFrom(SqlHelper, o))
                    .ToList();
        }

        public override void FinishInitialization()
        {
            foreach (var userPrivilege in UserPrivileges.Where(p => !p.IsSystemGeneratedObject))
            {
                switch (userPrivilege.ObjectType)
                {
                    case ObjectType.RELATION:
                    case ObjectType.VIEW:
                        userPrivilege.Relation =
                            Metadata
                                .MetadataRelations
                                .Relations[userPrivilege.ObjectName];
                        break;
                    case ObjectType.PROCEDURE:
                        userPrivilege.Procedure =
                            Metadata
                                .MetadataProcedures
                                .ProceduresByName[userPrivilege.ObjectName];
                        break;
                    case ObjectType.EXCEPTION:
                        userPrivilege.DbException =
                            Metadata
                                .MetadataExceptions
                                .ExceptionsByName[userPrivilege.ObjectName];
                        break;
                    case ObjectType.FIELD:
                        userPrivilege.Field =
                            Metadata
                                .MetadataFields
                                .Fields[userPrivilege.ObjectName];
                        break;
                    case ObjectType.CHARACTER_SET:
                        userPrivilege.CharacterSet =
                            Metadata
                                .MetadataCharacterSets
                                .CharacterSetsByName[userPrivilege.ObjectName];
                        break;
                    case ObjectType.ROLE:
                        userPrivilege.Role =
                            Metadata
                                .MetadataRoles
                                .Roles[userPrivilege.ObjectName];
                        break;
                    case ObjectType.GENERATOR:
                        userPrivilege.Generator =
                            Metadata
                                .MetadataGenerators
                                .GeneratorsByName[userPrivilege.ObjectName];
                        break;
                    case ObjectType.UDF:
                        userPrivilege.Function =
                            Metadata
                                .MetadataFunctions
                                .FunctionsByName[userPrivilege.LegacyFunctionNameKey];
                        break;
                    case ObjectType.COLLATION:
                        userPrivilege.Collation =
                            Metadata
                                .MetadataCollations
                                .CollationsByName[userPrivilege.ObjectName];
                        break;
                }
            }
        }

        public IEnumerable<CommandGroup> HandleUserPrivileges(IMetadata other, IComparerContext context)
        {
            if (context.Settings.IgnorePermissions)
            {
                yield break;
            }

            var toRevoke = other.MetadataUserPrivileges.UserPrivileges.Except(UserPrivileges);
            var toGrant = UserPrivileges.Except(other.MetadataUserPrivileges.UserPrivileges);
            var data =
                toRevoke
                    .Select(
                        x => new
                             {
                                 Privilege = x,
                                 Revoke = true
                             })
                    .Concat(toGrant.Select(
                                x => new
                                     {
                                         Privilege = x,
                                         Revoke = false
                                     }))
                    .GroupBy(x => x.Privilege.ObjectName);

            foreach (var group in data)
            {
                var result = new CommandGroup();
                foreach (var item in group)
                {
                    var privilege = item.Privilege;
                    var command =
                        item.Revoke
                            ? CanCreateRevoke(privilege, context)
                                  ? CreateRevoke(privilege, context)
                                  : null
                            : CanCreateGrant(privilege, context)
                                ? CreateGrant(privilege, context)
                                : null;
                    if (command != null && !command.IsEmpty)
                    {
                        result.Append(command);
                    }
                }
                if (!result.IsEmpty)
                {
                    yield return result;
                }
            }
        }

        protected virtual void AddWithOption(UserPrivilege privilege, Command command)
        {
            if (command != null && !command.IsEmpty && privilege.GrantOption)
            {
                command.AppendLine();
                command.Append(
                    privilege.Privilege == Privilege.MEMBER
                        ? "  WITH ADMIN OPTION"
                        : "  WITH GRANT OPTION");
            }
        }

        protected virtual void AddGrantedBy(UserPrivilege privilege, Command command)
        {
            if (command != null && !command.IsEmpty)
            {
                command.AppendLine();
                command.Append($"  GRANTED BY {privilege.Grantor.AsSqlIndentifier()}");
            }
        }

        protected virtual Command CreateGrant(UserPrivilege privilege, IComparerContext context)
        {
            var command = new Command();
            command.Append(
                privilege.Privilege == Privilege.MEMBER
                    ? $"GRANT {privilege.ObjectName.AsSqlIndentifier()} TO {privilege.User.AsSqlIndentifier()}"
                    : $"GRANT {CreatePrivilegeName(privilege)} ON {privilege.ObjectType.ToDescription()} {privilege.ObjectName.AsSqlIndentifier()} TO {CreateToObjectName(privilege)}");
            AddWithOption(privilege, command);
            AddGrantedBy(privilege, command);
            return command;
        }

        protected virtual Command CreateRevoke(UserPrivilege privilege, IComparerContext context)
        {
            var command = new Command();
            command.Append(
                privilege.Privilege == Privilege.MEMBER
                    ? $"REVOKE {privilege.ObjectName.AsSqlIndentifier()} FROM {privilege.User.AsSqlIndentifier()}"
                    : $"REVOKE {CreatePrivilegeName(privilege)} ON {privilege.ObjectType.ToDescription()} {privilege.ObjectName.AsSqlIndentifier()} FROM {CreateToObjectName(privilege)}");
            return command;
        }

        protected virtual bool CanCreateGrant(UserPrivilege privilege, IComparerContext context) => true;

        protected virtual bool CanCreateRevoke(UserPrivilege privilege, IComparerContext context)
        {
            ITypeObjectNameKey primitiveType;
            switch (privilege.ObjectType)
            {
                case ObjectType.RELATION:
                case ObjectType.VIEW:
                    primitiveType = privilege.Relation;
                    break;
                case ObjectType.PROCEDURE:
                    primitiveType = privilege.Procedure;
                    break;
                default:
                    primitiveType = null;
                    break;
            }
            return primitiveType == null || !context.DroppedObjects.Contains(primitiveType.TypeObjectNameKey);
        }

        protected virtual string CreatePrivilegeName(UserPrivilege userPrivilege)
        {
            var builder = new StringBuilder();
            builder.Append(userPrivilege.Privilege.ToDescription());
            if (userPrivilege.FieldName != null)
            {
                builder.Append($"({userPrivilege.FieldName.AsSqlIndentifier()})");
            }
            return builder.ToString();
        }

        protected virtual string CreateToObjectName(UserPrivilege userPrivilege)
        {
            var builder = new StringBuilder();
            if (!(userPrivilege.UserType == ObjectType.USER || userPrivilege.UserType == ObjectType.ROLE))
            {
                builder.Append(userPrivilege.UserType.ToDescription());
                builder.Append(" ");
            }
            builder.Append(userPrivilege.User.AsSqlIndentifier());
            return builder.ToString();
        }
    }
}
