using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.Implementations;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Compare
{
    partial class Comparer
    {
        public static Comparer ForTwoDatabases(IComparerSettings settings, string sourceConnectionString, string targetConnectionString)
        {
            var sqlHelper = GetSqlHelper(settings.TargetVersion);
            var databaseObjectFactory = GetDatabaseObjectFactory(settings.TargetVersion, sqlHelper);
            var sourceMetadata = new Metadata(sourceConnectionString, databaseObjectFactory);
            var targetMetadata = new Metadata(targetConnectionString, databaseObjectFactory);
            Parallel.Invoke(
                sourceMetadata.Initialize,
                targetMetadata.Initialize);
            return new Comparer(
                new ScriptBuilder(sqlHelper),
                new ComparerContext(settings),
                sourceMetadata,
                targetMetadata);
        }

        private static ISqlHelper GetSqlHelper(TargetVersion targetVersion) => targetVersion switch
        {
            TargetVersion.Version25 => new SqlHelper25(),
            TargetVersion.Version30 => new SqlHelper30(),
            TargetVersion.Version40 => new SqlHelper40(),
            _ => throw new ArgumentOutOfRangeException(nameof(targetVersion)),
        };

        private static Func<Metadata, IReadOnlyCollection<IDatabaseObject>> GetDatabaseObjectFactory(TargetVersion targetVersion, ISqlHelper sqlHelper) => targetVersion switch
        {
            TargetVersion.Version25 => m => DatabaseObjectFactory25(m, sqlHelper),
            TargetVersion.Version30 => m => DatabaseObjectFactory30(m, sqlHelper),
            TargetVersion.Version40 => m => DatabaseObjectFactory40(m, sqlHelper),
            _ => throw new ArgumentOutOfRangeException(nameof(targetVersion)),
        };

        private static IReadOnlyCollection<IDatabaseObject> DatabaseObjectFactory40(Metadata m, ISqlHelper sqlHelper)
        {
            return new IDatabaseObject[]
            {
                new MetadataCollations40(m, sqlHelper),
                new MetadataConstraints40(m, sqlHelper),
                new MetadataDatabase40(m, sqlHelper),
                new MetadataDependencies40(m, sqlHelper),
                new MetadataExceptions40(m, sqlHelper),
                new MetadataFields40(m, sqlHelper),
                new MetadataFunctions40(m, sqlHelper),
                new MetadataGenerators40(m, sqlHelper),
                new MetadataCharacterSets40(m, sqlHelper),
                new MetadataIndices40(m, sqlHelper),
                new MetadataPackages40(m, sqlHelper),
                new MetadataProcedures40(m, sqlHelper),
                new MetadataRelations40(m, sqlHelper),
                new MetadataRoles40(m, sqlHelper),
                new MetadataTriggers40(m, sqlHelper),
                new MetadataUserPrivileges40(m, sqlHelper),
            };
        }
        private static IReadOnlyCollection<IDatabaseObject> DatabaseObjectFactory30(Metadata m, ISqlHelper sqlHelper)
        {
            return new IDatabaseObject[]
            {
                new MetadataCollations30(m, sqlHelper),
                new MetadataConstraints30(m, sqlHelper),
                new MetadataDatabase30(m, sqlHelper),
                new MetadataDependencies30(m, sqlHelper),
                new MetadataExceptions30(m, sqlHelper),
                new MetadataFields30(m, sqlHelper),
                new MetadataFunctions30(m, sqlHelper),
                new MetadataGenerators30(m, sqlHelper),
                new MetadataCharacterSets30(m, sqlHelper),
                new MetadataIndices30(m, sqlHelper),
                new MetadataPackages30(m, sqlHelper),
                new MetadataProcedures30(m, sqlHelper),
                new MetadataRelations30(m, sqlHelper),
                new MetadataRoles30(m, sqlHelper),
                new MetadataTriggers30(m, sqlHelper),
                new MetadataUserPrivileges30(m, sqlHelper),
            };
        }
        private static IReadOnlyCollection<IDatabaseObject> DatabaseObjectFactory25(Metadata m, ISqlHelper sqlHelper)
        {
            return new IDatabaseObject[]
            {
                new MetadataCollations25(m, sqlHelper),
                new MetadataConstraints25(m, sqlHelper),
                new MetadataDatabase25(m, sqlHelper),
                new MetadataDependencies25(m, sqlHelper),
                new MetadataExceptions25(m, sqlHelper),
                new MetadataFields25(m, sqlHelper),
                new MetadataFunctions25(m, sqlHelper),
                new MetadataGenerators25(m, sqlHelper),
                new MetadataCharacterSets25(m, sqlHelper),
                new MetadataIndices25(m, sqlHelper),
                new MetadataProcedures25(m, sqlHelper),
                new MetadataRelations25(m, sqlHelper),
                new MetadataRoles25(m, sqlHelper),
                new MetadataTriggers25(m, sqlHelper),
                new MetadataUserPrivileges25(m, sqlHelper),
            };
        }
    }
}
