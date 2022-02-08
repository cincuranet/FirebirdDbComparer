namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing;

public class ComputedColumnToSameComputedColumnRawDataTypeFromOtherTable : ComparerTests.TestCaseStructure
{
    public override string Source => @"
create table parent (
    id bigint not null,
    code bigint
);
create table foobar (
    id bigint not null,
    id_master bigint not null,
    code computed by ((select code from parent where id = id_master))
);				
";

    public override string Target => @"
create table parent (
    id bigint not null,
    code int
);
create table foobar (
    id bigint not null,
    id_master bigint not null,
    code computed by ((select code from parent where id=id_master))
);
";
}
