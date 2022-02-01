using System.Collections.Generic;
using System.Diagnostics;

using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects;

[DebuggerDisplay("{PrimitiveTypeKey} CountOfDependencies:{Dependencies.Count}")]
public sealed class TreeNode
{
    private readonly ITypeObjectNameKey m_PrimitiveTypeKey;
    private readonly HashSet<ITypeObjectNameKey> m_Dependencies;
    private readonly HashSet<TreeNode> m_Nodes;

    public TreeNode(ITypeObjectNameKey primitiveTypeKey, HashSet<ITypeObjectNameKey> dependencies)
    {
        m_PrimitiveTypeKey = primitiveTypeKey;
        m_Dependencies = dependencies;
        m_Nodes = new HashSet<TreeNode>();
    }

    public ITypeObjectNameKey PrimitiveTypeKey => m_PrimitiveTypeKey;

    public HashSet<TreeNode> Nodes => m_Nodes;

    public HashSet<ITypeObjectNameKey> Dependencies => m_Dependencies;
}
