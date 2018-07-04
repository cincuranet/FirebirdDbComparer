using System.Collections.Generic;

namespace FirebirdDbComparer.Interfaces
{
    public interface IMetadata
    {
        string ConnectionString { get; }
        void Initialize();
        T GetSpecificDatabaseObject<T>() where T : IDatabaseObject;
        IReadOnlyCollection<IDatabaseObject> DatabaseObjects { get; }

        IMetadataCollations MetadataCollations { get; }
        IMetadataConstraints MetadataConstraints { get; }
        IMetadataDatabase MetadataDatabase { get; }
        IMetadataDependencies MetadataDependencies { get; }
        IMetadataExceptions MetadataExceptions { get; }
        IMetadataFields MetadataFields { get; }
        IMetadataFunctions MetadataFunctions { get; }
        IMetadataGenerators MetadataGenerators { get; }
        IMetadataCharacterSets MetadataCharacterSets { get; }
        IMetadataIndices MetadataIndices { get; }
        IMetadataProcedures MetadataProcedures { get; }
        IMetadataRelations MetadataRelations { get; }
        IMetadataRoles MetadataRoles { get; }
        IMetadataTriggers MetadataTriggers { get; }
        IMetadataUserPrivileges MetadataUserPrivileges { get; }
        IMetadataPackages MetadataPackages { get; }
    }
}
