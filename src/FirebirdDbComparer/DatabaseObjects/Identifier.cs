using System;
using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects;

public sealed class Identifier : DatabaseStringOrdinal
{
    public static IList<Identifier> EmptyIdentifierList = new List<Identifier>();

    private readonly bool m_IsComposite;

    public ISqlHelper SqlHelper { get; }

    public Identifier(ISqlHelper sqlHelper, string identifier)
        : base(identifier)
    {
        m_IsComposite = false;
        SqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof(sqlHelper));
    }

    public Identifier(ISqlHelper sqlHelper, params Identifier[] identifiers)
        : this(sqlHelper, BuildIdentifier(identifiers))
    {
        m_IsComposite = true;
    }

    public string AsSqlIndentifier() => !m_IsComposite ? SqlHelper.QuoteIdentifierIfNeeded(m_Value) : throw new InvalidOperationException();

    private static string BuildIdentifier(params Identifier[] identifiers) => string.Join(".", identifiers.Where(x => x != null));
}
