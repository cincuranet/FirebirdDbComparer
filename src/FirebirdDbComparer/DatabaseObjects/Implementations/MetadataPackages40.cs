using System;
using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations
{
    public class MetadataPackages40 : MetadataPackages30
    {
        public MetadataPackages40(IMetadata metadata, ISqlHelper sqlHelper)
            : base(metadata, sqlHelper)
        { }
    }
}
