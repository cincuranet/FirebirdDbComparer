using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations
{
    public class MetadataGenerators25 : DatabaseObject, IMetadataGenerators, ISupportsComment
    {
        private IDictionary<int, Generator> m_GeneratorsById;
        private IDictionary<Identifier, Generator> m_GeneratorsByName;

        public MetadataGenerators25(IMetadata metadata, ISqlHelper sqlHelper)
            : base(metadata, sqlHelper)
        { }

        public IDictionary<int, Generator> GeneratorsById => m_GeneratorsById;

        public IDictionary<Identifier, Generator> GeneratorsByName => m_GeneratorsByName;

        protected virtual string CommandText => @"
select trim(G.RDB$GENERATOR_NAME) as RDB$GENERATOR_NAME,
       G.RDB$GENERATOR_ID,
       G.RDB$SYSTEM_FLAG,
       G.RDB$DESCRIPTION
  from RDB$GENERATORS G";
        public override void Initialize()
        {
            var generators = Execute(CommandText)
                .Select(o => Generator.CreateFrom(SqlHelper, o))
                .ToArray();
            m_GeneratorsById = generators.ToDictionary(x => x.GeneratorId);
            m_GeneratorsByName = generators.ToDictionary(x => x.GeneratorName);
        }

        public override void FinishInitialization()
        { }

        IEnumerable<CommandGroup> ISupportsComment.Handle(IMetadata other, IComparerContext context)
        {
            var batch =
                new CommandGroup()
                    .Append(HandleComment(GeneratorsByName, other.MetadataGenerators.GeneratorsByName, x => x.GeneratorName, "SEQUENCE", x => new[] { x.GeneratorName }, context));

            if (!batch.IsEmpty)
            {
                yield return batch;
            }
        }

        public IEnumerable<CommandGroup> CreateGenerators(IMetadata other, IComparerContext context)
        {
            return FilterSystemFlagUser(GeneratorsByName.Values)
                .Where(g => !other.MetadataGenerators.GeneratorsByName.ContainsKey(g.GeneratorName))
                .Select(g => new CommandGroup().Append(g.Create(Metadata, other, context)));
        }

        public IEnumerable<CommandGroup> DropGenerators(IMetadata other, IComparerContext context)
        {
            return FilterSystemFlagUser(other.MetadataGenerators.GeneratorsByName.Values)
                .Where(g => !GeneratorsByName.ContainsKey(g.GeneratorName))
                .Select(g => new CommandGroup().Append(g.Drop(Metadata, other, context)));
        }

        public IEnumerable<CommandGroup> AlterGenerators(IMetadata other, IComparerContext context)
        {
            return FilterSystemFlagUser(GeneratorsByName.Values)
                .Where(g => other.MetadataGenerators.GeneratorsByName.TryGetValue(g.GeneratorName, out var otherGenerator) && g != otherGenerator)
                .Select(g => new CommandGroup().Append(g.Alter(Metadata, other, context)))
                .Where(x => !x.IsEmpty);
        }
    }
}
