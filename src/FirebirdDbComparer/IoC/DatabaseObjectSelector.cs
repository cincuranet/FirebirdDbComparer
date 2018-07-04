using System;

using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.IoC
{
    public class DatabaseObjectSelector : VersionHandlerSelector<IDatabaseObject>
    {
        public DatabaseObjectSelector(IComparerSettings comparerSettings)
            : base(comparerSettings)
        { }
    }
}
