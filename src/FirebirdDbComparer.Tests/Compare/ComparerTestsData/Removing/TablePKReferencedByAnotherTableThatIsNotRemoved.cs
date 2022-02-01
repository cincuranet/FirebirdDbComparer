using System;
using System.Linq;
using FirebirdDbComparer.Compare;
using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class TablePKReferencedByAnotherTableThatIsNotRemoved : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table detail (id int primary key, fk int);				
";

    public override string Target => @"
create table master (id int primary key);
create table detail (id int primary key, fk int references master(id));
";
}
