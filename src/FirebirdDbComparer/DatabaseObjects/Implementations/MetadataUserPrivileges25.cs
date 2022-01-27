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
                if (userPrivilege.ObjectType.IsRelation || userPrivilege.ObjectType.IsView)
                {
                    userPrivilege.Relation =
                        Metadata
                            .MetadataRelations
                            .Relations[userPrivilege.ObjectName];
                }
                else if (userPrivilege.ObjectType.IsProcedure)
                {
                    userPrivilege.Procedure =
                        Metadata
                            .MetadataProcedures
                            .ProceduresByName[userPrivilege.ObjectName];
                }
                else if (userPrivilege.ObjectType.IsException)
                {
                    userPrivilege.DbException =
                        Metadata
                            .MetadataExceptions
                            .ExceptionsByName[userPrivilege.ObjectName];
                }
                else if (userPrivilege.ObjectType.IsField)
                {
                    userPrivilege.Field =
                        Metadata
                            .MetadataFields
                            .Fields[userPrivilege.ObjectName];
                }
                else if (userPrivilege.ObjectType.IsCharacterSet)
                {
                    userPrivilege.CharacterSet =
                        Metadata
                            .MetadataCharacterSets
                            .CharacterSetsByName[userPrivilege.ObjectName];
                }
                else if (userPrivilege.ObjectType.IsRole)
                {
                    userPrivilege.Role =
                        Metadata
                            .MetadataRoles
                            .Roles[userPrivilege.ObjectName];
                }
                else if (userPrivilege.ObjectType.IsGenerator)
                {
                    userPrivilege.Generator =
                        Metadata
                            .MetadataGenerators
                            .GeneratorsByName[userPrivilege.ObjectName];
                }
                else if (userPrivilege.ObjectType.IsUDF)
                {
                    userPrivilege.Function =
                        Metadata
                            .MetadataFunctions
                            .FunctionsByName[userPrivilege.ObjectName];
                }
                else if (userPrivilege.ObjectType.IsCollation)
                {
                    userPrivilege.Collation =
                        Metadata
                            .MetadataCollations
                            .CollationsByName[userPrivilege.ObjectName];
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
                    privilege.Privilege == Privilege.Member
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
                privilege.Privilege == Privilege.Member
                    ? $"GRANT {privilege.ObjectName.AsSqlIndentifier()} TO {privilege.User.AsSqlIndentifier()}"
                    : $"GRANT {CreatePrivilegeName(privilege)} ON {privilege.ObjectType.ToSqlObject()} {privilege.ObjectName.AsSqlIndentifier()} TO {CreateToObjectName(privilege)}");
            AddWithOption(privilege, command);
            AddGrantedBy(privilege, command);
            return command;
        }

        protected virtual Command CreateRevoke(UserPrivilege privilege, IComparerContext context)
        {
            var command = new Command();
            command.Append(
                privilege.Privilege == Privilege.Member
                    ? $"REVOKE {privilege.ObjectName.AsSqlIndentifier()} FROM {privilege.User.AsSqlIndentifier()}"
                    : $"REVOKE {CreatePrivilegeName(privilege)} ON {privilege.ObjectType.ToSqlObject()} {privilege.ObjectName.AsSqlIndentifier()} FROM {CreateToObjectName(privilege)}");
            return command;
        }

        protected virtual bool CanCreateGrant(UserPrivilege privilege, IComparerContext context) => true;

        protected virtual bool CanCreateRevoke(UserPrivilege privilege, IComparerContext context)
        {
            ITypeObjectNameKey primitiveType;
            if (privilege.ObjectType.IsRelation || privilege.ObjectType.IsView)
            {
                primitiveType = privilege.Relation;
            }
            else if (privilege.ObjectType.IsProcedure)
            {
                primitiveType = privilege.Procedure;
            }
            else
            {
                return true;
            }
            return !context.DroppedObjects.Contains(primitiveType.TypeObjectNameKey);
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
            if (!(userPrivilege.UserType.IsUser || userPrivilege.UserType.IsRole))
            {
                builder.Append(userPrivilege.UserType.ToSqlObject());
                builder.Append(" ");
            }
            builder.Append(userPrivilege.User.AsSqlIndentifier());
            return builder.ToString();
        }
    }
}
