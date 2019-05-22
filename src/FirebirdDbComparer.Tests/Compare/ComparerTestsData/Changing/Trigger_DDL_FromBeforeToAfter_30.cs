using System;
using FirebirdDbComparer.Exceptions;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public class Trigger_DDL_FromBeforeToAfter_30 : ComparerTests.TestCaseSpecificAsserts
    {
        public override Type ExpectedCompareException => typeof(NotSupportedOnFirebirdException);
    }
}
