using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataTriggers40 : MetadataTriggers30
{
    public MetadataTriggers40(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }
}
