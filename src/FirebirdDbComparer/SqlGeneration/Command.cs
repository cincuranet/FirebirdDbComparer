using System.Text;

using FirebirdDbComparer.DatabaseObjects;

namespace FirebirdDbComparer.SqlGeneration;

public class Command
{
    private readonly StringBuilder m_Builder;

    public Command()
    {
        m_Builder = new StringBuilder();
    }

    public override string ToString()
    {
        return m_Builder.ToString();
    }

    public Command Append(string value)
    {
        m_Builder.Append(value);
        return this;
    }

    public Command Append(DatabaseStringOrdinal value)
    {
        return Append(value.ToString());
    }

    public Command AppendLine()
    {
        m_Builder.AppendLine();
        return this;
    }

    public bool IsEmpty => m_Builder.Length == 0;
}
