using System;
using System.Collections.Generic;

using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects
{
    public sealed class Identifier : DatabaseStringOrdinal
    {
        public static IList<Identifier> EmptyIdentifierList = new List<Identifier>();

        public ISqlHelper SqlHelper { get; }

        public Identifier(ISqlHelper sqlHelper, string identifier, string package = null)
            : base(BuildIdentifier(package, identifier))
        {
            SqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof(sqlHelper));
        }

#warning Proper handle PackageName
        public string AsSqlIndentifier() => SqlHelper.QuoteIdentifierIfNeeded(m_Value);

        private static string BuildIdentifier(string package, string identifier) => package != null ? $"{package}.{identifier}" : identifier;
    }
}
