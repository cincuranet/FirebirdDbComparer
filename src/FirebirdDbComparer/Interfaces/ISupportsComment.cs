using System.Collections.Generic;

using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Interfaces;

internal interface ISupportsComment
{
    IEnumerable<CommandGroup> Handle(IMetadata other, IComparerContext context);
}
