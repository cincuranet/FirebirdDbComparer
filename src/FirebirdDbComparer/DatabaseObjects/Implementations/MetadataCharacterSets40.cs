using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Implementations
{
    public class MetadataCharacterSets40 : MetadataCharacterSets30
    {
        public MetadataCharacterSets40(IMetadata metadata, ISqlHelper sqlHelper)
            : base(metadata, sqlHelper)
        { }
    }
}
