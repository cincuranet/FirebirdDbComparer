using System;

using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects;

public abstract class SqlElement<T> : BasicElement<T> where T : class
{
    public ISqlHelper SqlHelper { get; }

    protected SqlElement(ISqlHelper sqlHelper)
    {
        SqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof(sqlHelper));
    }
}
