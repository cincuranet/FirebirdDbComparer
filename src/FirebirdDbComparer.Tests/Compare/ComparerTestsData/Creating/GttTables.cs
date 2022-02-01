using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating;

public class GttTables : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create global temporary table gtt_delete (i int) on commit delete rows;
create global temporary table gtt_preserve (i int) on commit preserve rows;				
";

    public override string Target => @"

";
}
