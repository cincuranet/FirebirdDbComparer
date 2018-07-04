using System;
using System.Linq;

using Castle.MicroKernel;

using FirebirdDbComparer.Compare;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.IoC
{
    public abstract class VersionHandlerSelector<T> : IHandlerSelector
    {
        private readonly string m_Suffix;

        public IComparerSettings ComparerSettings { get; }

        public VersionHandlerSelector(IComparerSettings comparerSettings)
        {
            ComparerSettings = comparerSettings ?? throw new ArgumentNullException(nameof(comparerSettings));
            m_Suffix = ComparerSettings.TargetVersion.VersionSuffix();
        }

        public bool HasOpinionAbout(string key, Type service)
        {
            return service == typeof(T);
        }

        public IHandler SelectHandler(string key, Type service, IHandler[] handlers)
        {
            return handlers.SingleOrDefault(h => h.ComponentModel.Implementation.Name.EndsWith(m_Suffix));
        }
    }
}
