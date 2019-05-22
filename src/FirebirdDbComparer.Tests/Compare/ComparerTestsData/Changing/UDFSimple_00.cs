using System;
using FirebirdDbComparer.Exceptions;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
	public class UDFSimple_00 : ComparerTests.TestCaseSpecificAsserts
	{
		public override Type ExpectedCompareException => typeof(NotSupportedOnFirebirdException);
	}
}
