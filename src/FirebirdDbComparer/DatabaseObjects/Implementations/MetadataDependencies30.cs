using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataDependencies30 : MetadataDependencies25
{
    public MetadataDependencies30(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    protected override string CommandText => @"
select trim(D.RDB$DEPENDENT_NAME) as RDB$DEPENDENT_NAME,
       trim(D.RDB$DEPENDED_ON_NAME) as RDB$DEPENDED_ON_NAME,
       trim(D.RDB$FIELD_NAME) as RDB$FIELD_NAME,
       D.RDB$DEPENDENT_TYPE,
       D.RDB$DEPENDED_ON_TYPE,
       trim(D.RDB$PACKAGE_NAME) as RDB$PACKAGE_NAME
 from RDB$DEPENDENCIES D";

    public override void FinishInitialization()
    {
        // TODO: Danny, probably we should copy the code of the base class.
        // I will expected some problems with FUNCTIONS, we have now an additional PackageName as
        // namespace.
        base.FinishInitialization();

        // TODO: Danny, I'm not sure of this
        foreach (var dependency in Dependencies)
        {
            if (dependency.DependentType.IsPackageBody)
            {
                dependency.DependentPackage =
                    Metadata
                        .MetadataPackages
                        .PackagesByName[dependency.DependentNameKey];
            }

            if (dependency.DependedOnType.IsPackage)
            {
                dependency.DependendOnPackage =
                    Metadata
                        .MetadataPackages
                        .PackagesByName[dependency.DependedOnNameKey];
            }
        }
    }
}
