using System.Collections.Generic;
using System.Linq;
using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.Elements;

namespace FirebirdDbComparer.Interfaces
{
    public interface IMetadataDependencies : IDatabaseObject
    {
        IDictionary<Identifier, IList<Dependency>> DependedOnNames { get; }
        IDictionary<Identifier, IList<Dependency>> DependentNames { get; }
        List<Dependency> Dependencies { get; }
        TreeNode GetDependenciesFor(ITypeObjectNameKey primitiveTypeKey);
    }
}
