using System;

using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.IoC;

namespace FirebirdDbComparer.Ioc;

public class SqlHelperSelector : VersionHandlerSelector<ISqlHelper>
{
    public SqlHelperSelector(IComparerSettings comparerSettings)
        : base(comparerSettings)
    { }
}

