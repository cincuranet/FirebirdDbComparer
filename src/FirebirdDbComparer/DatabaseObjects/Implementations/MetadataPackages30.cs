using System;
using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataPackages30 : DatabaseObject, IMetadataPackages, ISupportsComment
{
    private IDictionary<Identifier, Package> m_PackagesByName;

    public MetadataPackages30(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    protected virtual string CommandText => @"
select trim(P.RDB$PACKAGE_NAME) as RDB$PACKAGE_NAME,
       P.RDB$PACKAGE_HEADER_SOURCE,
       P.RDB$PACKAGE_BODY_SOURCE,
       P.RDB$VALID_BODY_FLAG,
       trim(P.RDB$OWNER_NAME) as RDB$OWNER_NAME,
       P.RDB$SYSTEM_FLAG,
       P.RDB$DESCRIPTION
  from RDB$PACKAGES P";

    public IDictionary<Identifier, Package> PackagesByName => m_PackagesByName;

    public override void Initialize()
    {
        var packages =
            Execute(CommandText)
                .Select(o => Package.CreateFrom(SqlHelper, o))
                .ToArray();
        m_PackagesByName = packages.ToDictionary(x => x.PackageName);
    }

    public override void FinishInitialization()
    { }

    IEnumerable<CommandGroup> ISupportsComment.Handle(IMetadata other, IComparerContext context)
    {
        var result = new CommandGroup().Append(HandleComment(PackagesByName, other.MetadataPackages.PackagesByName, x => x.PackageName, "PACKAGE", x => new[] { x.PackageName }, context));
        if (!result.IsEmpty)
        {
            yield return result;
        }
    }

    public IEnumerable<CommandGroup> CreateNewPackagesHeaders(IMetadata other, IComparerContext context)
    {
        return FilterNewPackages(other)
            .Select(package => new CommandGroup().Append(WrapActionWithEmptyBody(package.Create)(Metadata, other, context)));
    }

    public IEnumerable<CommandGroup> CreateNewPackagesBodies(IMetadata other, IComparerContext context)
    {
        return FilterNewPackages(other)
            .Where(package => package.ValidBodyFlag)
            .Select(package => new CommandGroup().Append(package.Create(Metadata, other, context)));
    }

    public IEnumerable<CommandGroup> DropPackages(IMetadata other, IComparerContext context)
    {
        var complete = FilterCompletePackagesToBeDropped(other)
            .Select(package => new CommandGroup().Append(WrapActionWithEmptyBody(package.Drop)(Metadata, other, context)));
        var bodies = FilterPackagesBodiesToBeDropped(other)
            .Select(package => new CommandGroup().Append(package.Drop(Metadata, other, context)));
        return bodies.Concat(complete);
    }

    public IEnumerable<CommandGroup> AlterPackagesHeaders(IMetadata other, IComparerContext context)
    {
        return FilterPackagesHeadersToBeAltered(other)
            .Select(package => new CommandGroup().Append(WrapActionWithEmptyBody(package.Alter)(Metadata, other, context)));
    }

    public IEnumerable<CommandGroup> AlterPackagesBodies(IMetadata other, IComparerContext context)
    {
        return FilterPackagesBodiesToBeAltered(other)
            .Where(package => package.ValidBodyFlag)
            .Select(package => new CommandGroup()
                // force header re-sync header as "sometimes" the body fails with signature mismatch (CORE-6250)
                .Append(WrapActionWithEmptyBody(package.Alter)(Metadata, other, context))
                .Append(package.Alter(Metadata, other, context)));
    }

    protected virtual IEnumerable<Package> FilterNewPackages(IMetadata other)
    {
        return FilterSystemFlagUser(PackagesByName.Values)
            .Where(p => !other.MetadataPackages.PackagesByName.ContainsKey(p.PackageName));
    }

    protected virtual IEnumerable<Package> FilterCompletePackagesToBeDropped(IMetadata other)
    {
        return FilterSystemFlagUser(other.MetadataPackages.PackagesByName.Values)
            .Where(p => !PackagesByName.ContainsKey(p.PackageName));
    }

    protected virtual IEnumerable<Package> FilterPackagesBodiesToBeDropped(IMetadata other)
    {
        return FilterSystemFlagUser(PackagesByName.Values)
            .Where(p => other.MetadataPackages.PackagesByName.TryGetValue(p.PackageName, out var otherPackage)
                && otherPackage.PackageBodySource != null
                && p.PackageBodySource == null);
    }

    protected virtual IEnumerable<Package> FilterPackagesHeadersToBeAltered(IMetadata other)
    {
        return FilterSystemFlagUser(PackagesByName.Values)
            .Where(p => other.MetadataPackages.PackagesByName.TryGetValue(p.PackageName, out var otherPackage) && otherPackage != p
                && p.PackageHeaderSource != null
                && !Package.PackageHeaderComparer.Equals(p, otherPackage));
    }

    protected virtual IEnumerable<Package> FilterPackagesBodiesToBeAltered(IMetadata other)
    {
        return FilterSystemFlagUser(PackagesByName.Values)
            .Where(p => other.MetadataPackages.PackagesByName.TryGetValue(p.PackageName, out var otherPackage) && otherPackage != p
                && p.PackageBodySource != null
                && !Package.PackageBodyComparer.Equals(p, otherPackage));
    }
}
