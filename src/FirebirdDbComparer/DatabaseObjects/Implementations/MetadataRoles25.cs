using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations
{
    public class MetadataRoles25 : DatabaseObject, IMetadataRoles, ISupportsComment
    {
        private IDictionary<Identifier, Role> m_Roles;

        public MetadataRoles25(IMetadata metadata, ISqlHelper sqlHelper)
            : base(metadata, sqlHelper)
        { }

        public IDictionary<Identifier, Role> Roles => m_Roles;

        protected virtual string CommandText => @"
select trim(R.RDB$ROLE_NAME) as RDB$ROLE_NAME,
       trim(R.RDB$OWNER_NAME) as RDB$OWNER_NAME,
       R.RDB$DESCRIPTION,
       R.RDB$SYSTEM_FLAG
  from RDB$ROLES R";

        public override void Initialize()
        {
            m_Roles =
                Execute(CommandText)
                    .Select(o => Role.CreateFrom(SqlHelper, o))
                    .ToDictionary(x => x.RoleName);
        }

        public override void FinishInitialization()
        { }

        IEnumerable<CommandGroup> ISupportsComment.Handle(IMetadata other, IComparerContext context)
        {
            var result = new CommandGroup().Append(HandleComment(Roles, other.MetadataRoles.Roles, x => x.RoleName, "ROLE", x => new[] { x.RoleName }, context));
            if (!result.IsEmpty)
            {
                yield return result;
            }
        }

        public IEnumerable<CommandGroup> CreateRoles(IMetadata other, IComparerContext context)
        {
            return FilterSystemFlagUser(Roles.Values)
                .Where(r => !other.MetadataRoles.Roles.ContainsKey(r.RoleName))
                .Select(r => new CommandGroup().Append(r.Create(Metadata, other, context)));
        }

        public IEnumerable<CommandGroup> DropRoles(IMetadata other, IComparerContext context)
        {
            return FilterSystemFlagUser(other.MetadataRoles.Roles.Values)
                .Where(r => !Roles.ContainsKey(r.RoleName))
                .Select(r => new CommandGroup().Append(r.Drop(Metadata, other, context)));
        }

        public IEnumerable<CommandGroup> AlterRoles(IMetadata other, IComparerContext context)
        {
            return FilterSystemFlagUser(Roles.Values)
                .Where(r => other.MetadataRoles.Roles.TryGetValue(r.RoleName, out var otherRole) && r != otherRole)
                .Select(r => new CommandGroup().Append(r.Alter(Metadata, other, context)))
                .Where(x => !x.IsEmpty);
        }
    }
}
