using System;
using System.Collections.Generic;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Primitives
{
    public sealed class Package : Primitive<Package>, IHasSystemFlag, IHasDescription
    {
        private sealed class PackageHeaderEqualityComparer : PropertiesEqualityComparerBase<Package>
        {
            private static readonly EquatableProperty<Package>[] s_EquatableProperties =
            {
                new EquatableProperty<Package>(x => x.PackageHeaderSource, nameof(PackageHeaderSource))
            };

            public PackageHeaderEqualityComparer()
                : base(s_EquatableProperties)
            { }
        }

        private sealed class PackageBodyEqualityComparer : PropertiesEqualityComparerBase<Package>
        {
            private static readonly EquatableProperty<Package>[] s_EquatableProperties =
            {
                new EquatableProperty<Package>(x => x.PackageBodySource, nameof(PackageBodySource))
            };

            public PackageBodyEqualityComparer()
                : base(s_EquatableProperties)
            { }
        }

        private static readonly EquatableProperty<Package>[] s_EquatableProperties =
        {
            new EquatableProperty<Package>(x => x.PackageName, nameof(PackageName)),
            new EquatableProperty<Package>(x => x.PackageHeaderSource, nameof(PackageHeaderSource)),
            new EquatableProperty<Package>(x => x.PackageBodySource, nameof(PackageBodySource)),
            new EquatableProperty<Package>(x => x.OwnerName, nameof(OwnerName))
        };

        public Package(ISqlHelper sqlHelper)
            : base(sqlHelper)
        { }

        public static IEqualityComparer<Package> PackageHeaderComparer { get; } = new PackageHeaderEqualityComparer();
        public static IEqualityComparer<Package> PackageBodyComparer { get; } = new PackageBodyEqualityComparer();

        public Identifier PackageName { get; private set; }
        public DatabaseStringOrdinal PackageHeaderSource { get; private set; }
        public DatabaseStringOrdinal PackageBodySource { get; private set; }
        public bool ValidBodyFlag { get; private set; }
        public Identifier OwnerName { get; private set; }
        public SystemFlagType SystemFlag { get; private set; }
        public DatabaseStringOrdinal Description { get; private set; }

        protected override Package Self => this;

        protected override EquatableProperty<Package>[] EquatableProperties => s_EquatableProperties;

        protected override IEnumerable<Command> OnCreate(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            var command = new PSqlCommand();
            if (context.EmptyBodiesEnabled)
            {
                command.Append($"CREATE OR ALTER PACKAGE {PackageName.AsSqlIndentifier()}");
            }
            else
            {
                command.Append($"RECREATE PACKAGE BODY {PackageName.AsSqlIndentifier()}");
            }
            command.AppendLine();
            command.Append("AS");
            command.AppendLine();
            if (context.EmptyBodiesEnabled)
            {
                command.Append(PackageHeaderSource);
            }
            else
            {
                command.Append(PackageBodySource);
            }
            yield return command;
        }

        protected override IEnumerable<Command> OnDrop(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            if (context.EmptyBodiesEnabled)
            {
                yield return new Command()
                    .Append($"DROP PACKAGE {PackageName.AsSqlIndentifier()}");
            }
            else
            {
                yield return new Command()
                    .Append($"DROP PACKAGE BODY {PackageName.AsSqlIndentifier()}");
            }
        }

        protected override IEnumerable<Command> OnAlter(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            return OnCreate(sourceMetadata, targetMetadata, context);
        }

        protected override Identifier OnPrimitiveTypeKeyObjectName() => PackageName;

        public static Package CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values)
        {
            var result =
                new Package(sqlHelper)
                {
                    PackageName = new Identifier(sqlHelper, values["RDB$PACKAGE_NAME"].DbValueToString()),
                    PackageHeaderSource = values["RDB$PACKAGE_HEADER_SOURCE"].DbValueToString(),
                    PackageBodySource = values["RDB$PACKAGE_BODY_SOURCE"].DbValueToString(),
                    ValidBodyFlag = values["RDB$VALID_BODY_FLAG"].DbValueToFlag(),
                    OwnerName = new Identifier(sqlHelper, values["RDB$OWNER_NAME"].DbValueToString()),
                    SystemFlag = (SystemFlagType)values["RDB$SYSTEM_FLAG"].DbValueToInt32().GetValueOrDefault(),
                    Description = values["RDB$DESCRIPTION"].DbValueToString()
                };
            return result;
        }
    }
}
