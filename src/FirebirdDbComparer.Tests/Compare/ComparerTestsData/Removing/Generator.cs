using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class Generator : ComparerTests.TestCaseStructure
{
    public override string Source => @"
				
";

    public override string Target => @"
create sequence new_generator;
";
}
