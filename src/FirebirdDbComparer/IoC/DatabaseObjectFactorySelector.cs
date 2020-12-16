using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Castle.Facilities.TypedFactory;
using Castle.MicroKernel;

using FirebirdDbComparer.Compare;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.Ioc
{
    public class DatabaseObjectFactorySelector : DefaultTypedFactoryComponentSelector
    {
        private readonly string m_Suffix;
        public IComparerSettings ComparerSettings { get; }

        public DatabaseObjectFactorySelector(IComparerSettings comparerSettings)
        {
            ComparerSettings = comparerSettings ?? throw new ArgumentNullException(nameof(comparerSettings));
            m_Suffix = ComparerSettings.TargetVersion.VersionSuffix();
        }

        protected override Func<IKernelInternal, IReleasePolicy, object> BuildFactoryComponent(MethodInfo method, string componentName, Type componentType, Arguments additionalArguments)
        {
            return
                (k, s) =>
                {
                    var components = new HashSet<IDatabaseObject>(k.ResolveAll(typeof(IDatabaseObject), additionalArguments, s).Cast<IDatabaseObject>());
                    var filteredComponents =
                        components
                            .Where(o => o.GetType().Name.EndsWith(m_Suffix))
                            .ToArray();
                    components.ExceptWith(filteredComponents);
                    foreach (var item in components)
                    {
                        k.ReleaseComponent(item);
                    }
                    return filteredComponents;
                };
        }
    }
}
