using System;
using System.Collections.Generic;

using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects
{
    public sealed class Identifier : DatabaseStringOrdinal
    {
        public static IList<Identifier> EmptyIdentifierList = new List<Identifier>();

        public ISqlHelper SqlHelper { get; }

        public Identifier(ISqlHelper sqlHelper, string identifier)
            : base(identifier)
        {
            SqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof(sqlHelper));
        }

        public string AsSqlIndentifier() => SqlHelper.QuoteIdentifierIfNeeded(Value);
    }
}
