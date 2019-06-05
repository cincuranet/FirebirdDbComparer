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
                if (userPrivilege.ObjectType == ObjectType.Package)
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
                        command.Append($"GRANT USAGE ON {privilege.ObjectType.ToDescription()} {privilege.ObjectName.AsSqlIndentifier()} TO {privilege.User.AsSqlIndentifier()}");
                        AddWithOption(privilege, command);
                        AddGrantedBy(privilege, command);

                        // see \src\dsql\parse.y line 840 (release 3.0.2)
                        // only EXECEPTION, GENERATOR is parsed for the moment, but not clearly in the documentation
                        // reported 
                        if (privilege.ObjectType != ObjectType.Exception && privilege.ObjectType != ObjectType.Generator)
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
                            ? $"GRANT {privilege.Privilege.ToDescription()} {privilege.ObjectType.ToDescription()} TO {privilege.UserType.ToDescription()} {privilege.User}"
                            : $"GRANT {privilege.Privilege.ToDescription()} ANY {privilege.ObjectType.ToDescription()} TO {privilege.UserType.ToDescription()} {privilege.User}");
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
                        command.Append($"REVOKE USAGE ON {privilege.ObjectType.ToDescription()} {privilege.ObjectName.AsSqlIndentifier()} FROM {privilege.User.AsSqlIndentifier()}");

                        // see \src\dsql\parse.y line 840 (release 3.0.2)
                        // only EXECEPTION, GENERATOR is parsed for the moment, but not clearly in the documentation
                        // reported 
                        if (privilege.ObjectType != ObjectType.Exception && privilege.ObjectType != ObjectType.Generator)
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
                            ? $"REVOKE {privilege.Privilege.ToDescription()} {privilege.ObjectType.ToDescription()} FROM {privilege.UserType.ToDescription()} {privilege.User}"
                            : $"REVOKE {privilege.Privilege.ToDescription()} ANY {privilege.ObjectType.ToDescription()} FROM {privilege.UserType.ToDescription()} {privilege.User}");
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
            switch (privilege.ObjectType)
            {
                case ObjectType.Relation:
                case ObjectType.View:
                    primitiveType = privilege.Relation;
                    break;
                case ObjectType.Procedure:
                    primitiveType = privilege.Procedure;
                    break;
                case ObjectType.Package:
                    primitiveType = privilege.Package;
                    break;
                case ObjectType.UDF:
                    primitiveType = privilege.Function;
                    break;
                case ObjectType.Exception:
                    primitiveType = privilege.DbException;
                    break;
                case ObjectType.Generator:
                    primitiveType = privilege.Generator;
                    break;
                default:
                    return true;
            }
            return !context.DroppedObjects.Contains(primitiveType.TypeObjectNameKey);
        }
    }
}
