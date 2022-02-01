using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataDatabase40 : MetadataDatabase30
{
    public MetadataDatabase40(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }
}
