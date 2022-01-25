using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Changing
{
    public  class ComputedColumnToSameComputedColumnRawDataTypeWithCollationFromOtherTable : ComparerTests.TestCaseStructure
    {
        public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
        {
            return targetVersion.AtMost(TargetVersion.Version25);
        }

        public override string Source => @"
create table parent (
    id bigint not null,
    code varchar(512) character set utf8 not null collate unicode_ci
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
    code varchar(512) character set utf8 not null collate utf8
);
create table foobar (
    id bigint not null,
    id_master bigint not null,
    code computed by ((select code from parent where id=id_master))
);
";
    }
}
