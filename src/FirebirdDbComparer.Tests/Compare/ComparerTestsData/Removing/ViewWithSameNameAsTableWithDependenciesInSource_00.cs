using System;
using FirebirdDbComparer.Exceptions;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing
{
    public class ViewWithSameNameAsTableWithDependenciesInSource_00 : ComparerTests.TestCaseSpecificAsserts
    {
        public override Type ExpectedCompareException => typeof(CrossTypesOfSameObjectTypesException);
    }
}
