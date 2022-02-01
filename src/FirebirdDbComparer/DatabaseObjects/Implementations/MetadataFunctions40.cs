using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Exceptions;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataFunctions40 : MetadataFunctions30
{
    public MetadataFunctions40(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }
}
