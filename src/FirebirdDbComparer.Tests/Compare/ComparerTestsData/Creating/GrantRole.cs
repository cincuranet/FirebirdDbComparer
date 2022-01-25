using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Creating
{
    public class GrantRole : ComparerTests.TestCaseStructure
    {
        public override string Source => @"
create role test_role;
grant test_role to test_user;				
";

        public override string Target => @"
create role test_role;
";
    }
}
