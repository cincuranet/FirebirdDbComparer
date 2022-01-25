using System.Collections.Generic;
using System.Linq;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Implementations
{
    public class MetadataProcedures40 : MetadataProcedures30
    {
        public MetadataProcedures40(IMetadata metadata, ISqlHelper sqlHelper)
            : base(metadata, sqlHelper)
        { }
    }
}
