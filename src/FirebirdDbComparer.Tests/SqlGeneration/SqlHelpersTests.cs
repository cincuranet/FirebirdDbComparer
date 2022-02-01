using System.Collections.Generic;

using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

using NUnit.Framework;

namespace FirebirdDbComparer.Tests.SqlGeneration;

[TestFixture]
public class SqlHelpersTests
{
    static IEnumerable<TestCaseData> DoubleSingleQuotesSource()
    {
        yield return new TestCaseData(new SqlHelper25(), null).Returns(null);
        yield return new TestCaseData(new SqlHelper30(), null).Returns(null);
        yield return new TestCaseData(new SqlHelper40(), null).Returns(null);
        yield return new TestCaseData(new SqlHelper25(), "").Returns("");
        yield return new TestCaseData(new SqlHelper30(), "").Returns("");
        yield return new TestCaseData(new SqlHelper40(), "").Returns("");
        yield return new TestCaseData(new SqlHelper25(), "   ").Returns("   ");
        yield return new TestCaseData(new SqlHelper30(), "   ").Returns("   ");
        yield return new TestCaseData(new SqlHelper40(), "   ").Returns("   ");
        yield return new TestCaseData(new SqlHelper25(), "\t ").Returns("\t ");
        yield return new TestCaseData(new SqlHelper30(), "\t ").Returns("\t ");
        yield return new TestCaseData(new SqlHelper40(), "\t ").Returns("\t ");
        yield return new TestCaseData(new SqlHelper25(), "ji'ri").Returns("ji''ri");
        yield return new TestCaseData(new SqlHelper30(), "ji'ri").Returns("ji''ri");
        yield return new TestCaseData(new SqlHelper40(), "ji'ri").Returns("ji''ri");
        yield return new TestCaseData(new SqlHelper25(), "'ri").Returns("''ri");
        yield return new TestCaseData(new SqlHelper30(), "'ri").Returns("''ri");
        yield return new TestCaseData(new SqlHelper40(), "'ri").Returns("''ri");
        yield return new TestCaseData(new SqlHelper25(), "ji'").Returns("ji''");
        yield return new TestCaseData(new SqlHelper30(), "ji'").Returns("ji''");
        yield return new TestCaseData(new SqlHelper40(), "ji'").Returns("ji''");
        yield return new TestCaseData(new SqlHelper25(), "'").Returns("''");
        yield return new TestCaseData(new SqlHelper30(), "'").Returns("''");
        yield return new TestCaseData(new SqlHelper40(), "'").Returns("''");
    }
    [TestCaseSource(nameof(DoubleSingleQuotesSource))]
    public string DoubleSingleQuotes(ISqlHelper instance, string input)
    {
        return instance.DoubleSingleQuotes(input);
    }

    static IEnumerable<TestCaseData> QuoteIdentifierIfNeededSource()
    {
        yield return new TestCaseData(new SqlHelper25(), null).Returns(null);
        yield return new TestCaseData(new SqlHelper30(), null).Returns(null);
        yield return new TestCaseData(new SqlHelper40(), null).Returns(null);
        yield return new TestCaseData(new SqlHelper25(), "").Returns("");
        yield return new TestCaseData(new SqlHelper30(), "").Returns("");
        yield return new TestCaseData(new SqlHelper40(), "").Returns("");
        yield return new TestCaseData(new SqlHelper25(), "JIRI").Returns("JIRI");
        yield return new TestCaseData(new SqlHelper30(), "JIRI").Returns("JIRI");
        yield return new TestCaseData(new SqlHelper40(), "JIRI").Returns("JIRI");
        yield return new TestCaseData(new SqlHelper25(), "Jiri").Returns("\"Jiri\"");
        yield return new TestCaseData(new SqlHelper30(), "Jiri").Returns("\"Jiri\"");
        yield return new TestCaseData(new SqlHelper40(), "Jiri").Returns("\"Jiri\"");
        yield return new TestCaseData(new SqlHelper25(), "č").Returns("\"č\"");
        yield return new TestCaseData(new SqlHelper30(), "č").Returns("\"č\"");
        yield return new TestCaseData(new SqlHelper40(), "č").Returns("\"č\"");
        yield return new TestCaseData(new SqlHelper25(), "JI_RI").Returns("JI_RI");
        yield return new TestCaseData(new SqlHelper30(), "JI_RI").Returns("JI_RI");
        yield return new TestCaseData(new SqlHelper40(), "JI_RI").Returns("JI_RI");
        yield return new TestCaseData(new SqlHelper25(), "JI$RI").Returns("JI$RI");
        yield return new TestCaseData(new SqlHelper30(), "JI$RI").Returns("JI$RI");
        yield return new TestCaseData(new SqlHelper40(), "JI$RI").Returns("JI$RI");
        yield return new TestCaseData(new SqlHelper25(), "JI-RI").Returns("\"JI-RI\"");
        yield return new TestCaseData(new SqlHelper30(), "JI-RI").Returns("\"JI-RI\"");
        yield return new TestCaseData(new SqlHelper40(), "JI-RI").Returns("\"JI-RI\"");
        yield return new TestCaseData(new SqlHelper25(), "JI~RI").Returns("\"JI~RI\"");
        yield return new TestCaseData(new SqlHelper30(), "JI~RI").Returns("\"JI~RI\"");
        yield return new TestCaseData(new SqlHelper40(), "JI~RI").Returns("\"JI~RI\"");
        yield return new TestCaseData(new SqlHelper25(), "6JI").Returns("\"6JI\"");
        yield return new TestCaseData(new SqlHelper30(), "6JI").Returns("\"6JI\"");
        yield return new TestCaseData(new SqlHelper40(), "6JI").Returns("\"6JI\"");
        yield return new TestCaseData(new SqlHelper25(), "$JI").Returns("\"$JI\"");
        yield return new TestCaseData(new SqlHelper30(), "$JI").Returns("\"$JI\"");
        yield return new TestCaseData(new SqlHelper40(), "$JI").Returns("\"$JI\"");
        yield return new TestCaseData(new SqlHelper25(), "_JI").Returns("\"_JI\"");
        yield return new TestCaseData(new SqlHelper30(), "_JI").Returns("\"_JI\"");
        yield return new TestCaseData(new SqlHelper40(), "_JI").Returns("\"_JI\"");
    }
    [TestCaseSource(nameof(QuoteIdentifierIfNeededSource))]
    public string QuoteIdentifierIfNeeded(ISqlHelper instance, string input)
    {
        return instance.QuoteIdentifierIfNeeded(input);
    }
}
