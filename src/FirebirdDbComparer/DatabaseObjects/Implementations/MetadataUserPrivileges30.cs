using System;
using System.Linq;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations
{
    public class MetadataUserPrivileges30 : MetadataUserPrivileges25
    {
        public MetadataUserPrivileges30(IMetadata metadata, ISqlHelper sqlHelper)
            : base(metadata, sqlHelper)
        { }

        public override void FinishInitialization()
        {
            base.FinishInitialization();

            foreach (var userPrivilege in UserPrivileges.Where(p => !p.IsSystemGeneratedObject))
            {
                if (userPrivilege.ObjectType.IsPackage)
                {
                    userPrivilege.Package =
                        Metadata
                            .MetadataPackages
                            .PackagesByName[userPrivilege.ObjectName];
                }
            }
        }

        protected override Command CreateGrant(UserPrivilege privilege, IComparerContext context)
        {
            Command command;
            switch (privilege.Privilege)
            {
                case Privilege.Usage:
                {
                    // Syntax:
                    //   GRANT USAGE ON < object type > < name > TO < grantee list > [< grant option > < granted by clause >]
                    //   <object type> ::= {DOMAIN | EXCEPTION | GENERATOR | SEQUENCE | CHARACTER SET | COLLATION}

                    if (!privilege.IsSystemGeneratedObject)
                    {
                        command = new Command();
                        command.Append($"GRANT USAGE ON {privilege.ObjectType.ToSqlObject()} {privilege.ObjectName.AsSqlIndentifier()} TO {privilege.User.AsSqlIndentifier()}");
                        AddWithOption(privilege, command);
                        AddGrantedBy(privilege, command);

                        // see \src\dsql\parse.y line 840 (release 3.0.2)
                        // only EXECEPTION, GENERATOR is parsed for the moment, but not clearly in the documentation
                        // reported 
                        if (!privilege.ObjectType.IsException && !privilege.ObjectType.IsGenerator)
                        {
                            command = null;
                        }
                    }
                    else
                    {
                        command = null;
                    }
                    break;
                }
                case Privilege.Create:
                case Privilege.Alter:
                case Privilege.Drop:
                {
                    // Syntax:
                    //   GRANT CREATE <object-type >
                    //       TO[USER | ROLE] < user - name > | < role - name > [WITH GRANT OPTION];
                    //   GRANT ALTER ANY < object - type >
                    //       TO[USER | ROLE] < user - name > | < role - name > [WITH GRANT OPTION];
                    //   GRANT DROP ANY < object - type >
                    //       TO[USER | ROLE] < user - name > | < role - name > [WITH GRANT OPTION];

                    command = new Command();
                    command.Append(
                        privilege.Privilege == Privilege.Create
                            ? $"GRANT {privilege.Privilege.ToDescription()} {privilege.ObjectType.ToSqlObject()} TO {privilege.UserType.ToSqlObject()} {privilege.User}"
                            : $"GRANT {privilege.Privilege.ToDescription()} ANY {privilege.ObjectType.ToSqlObject()} TO {privilege.UserType.ToSqlObject()} {privilege.User}");
                    AddWithOption(privilege, command);
                    break;
                }
                default:
                    command = base.CreateGrant(privilege, context);
                    break;
            }
            return command;
        }

        protected override Command CreateRevoke(UserPrivilege privilege, IComparerContext context)
        {
            Command command;
            switch (privilege.Privilege)
            {
                case Privilege.Usage:
                {
                    // Syntax:
                    //   REVOKE USAGE ON < object type > < name > FROM < grantee list > [< granted by clause >]
                    //   <object type> ::= {DOMAIN | EXCEPTION | GENERATOR | SEQUENCE | CHARACTER SET | COLLATION}

                    if (!privilege.IsSystemGeneratedObject)
                    {
                        command = new Command();
                        command.Append($"REVOKE USAGE ON {privilege.ObjectType.ToSqlObject()} {privilege.ObjectName.AsSqlIndentifier()} FROM {privilege.User.AsSqlIndentifier()}");

                        // see \src\dsql\parse.y line 840 (release 3.0.2)
                        // only EXECEPTION, GENERATOR is parsed for the moment, but not clearly in the documentation
                        // reported 
                        if (!privilege.ObjectType.IsException && !privilege.ObjectType.IsGenerator)
                        {
                            command = null;
                        }
                    }
                    else
                    {
                        command = null;
                    }
                    break;
                }
                case Privilege.Create:
                case Privilege.Alter:
                case Privilege.Drop:
                {
                    // Syntax:
                    //   REVOKE[GRANT OPTION FOR] CREATE < object - type >
                    //       FROM[USER | ROLE] < user - name > | < role - name >;
                    //   REVOKE[GRANT OPTION FOR] ALTER ANY<object-type >
                    //       FROM[USER | ROLE] < user - name > | < role - name >;
                    //   REVOKE[GRANT OPTION FOR] DROP ANY<object-type >
                    //       FROM[USER | ROLE] < user - name > | < role - name >;

                    command = new Command();
                    command.Append(
                        privilege.Privilege == Privilege.Create
                            ? $"REVOKE {privilege.Privilege.ToDescription()} {privilege.ObjectType.ToSqlObject()} FROM {privilege.UserType.ToSqlObject()} {privilege.User}"
                            : $"REVOKE {privilege.Privilege.ToDescription()} ANY {privilege.ObjectType.ToSqlObject()} FROM {privilege.UserType.ToSqlObject()} {privilege.User}");
                    break;
                }
                default:
                    command = base.CreateRevoke(privilege, context);
                    break;
            }
            return command;
        }

        protected override bool CanCreateRevoke(UserPrivilege privilege, IComparerContext context)
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
            else if (privilege.ObjectType.IsPackage)
            {
                primitiveType = privilege.Package;
            }
            else if (privilege.ObjectType.IsUDF)
            {
                primitiveType = privilege.Function;
            }
            else if (privilege.ObjectType.IsException)
            {
                primitiveType = privilege.DbException;
            }
            else if (privilege.ObjectType.IsGenerator)
            {
                primitiveType = privilege.Generator;
            }
            else
            {
                return true;
            }
            return !context.DroppedObjects.Contains(primitiveType.TypeObjectNameKey);
        }
    }
}
