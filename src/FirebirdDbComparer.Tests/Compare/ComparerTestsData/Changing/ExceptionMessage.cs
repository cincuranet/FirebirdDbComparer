using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class ExceptionMessage : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create exception e 'new message';				
";

    public override string Target => @"
create exception e 'old message';
";
}
