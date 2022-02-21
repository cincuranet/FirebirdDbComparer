using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects;

internal class Metadata : IMetadata
{
    public Metadata(string connectionString, IDatabaseObjectFactory databaseObjectFactory)
    {
        ConnectionString = connectionString;
        DatabaseObjectFactory = databaseObjectFactory ?? throw new ArgumentNullException(nameof(databaseObjectFactory));
    }

    IReadOnlyCollection<IDatabaseObject> m_DatabaseObjects;
    private IMetadataCollations m_MetadataCollations;
    private IMetadataConstraints m_MetadataConstraints;
    private IMetadataDatabase m_MetadataDatabase;
    private IMetadataDependencies m_MetadataDependencies;
    private IMetadataExceptions m_MetadataExceptions;
    private IMetadataFields m_MetadataFields;
    private IMetadataFunctions m_MetadataFunctions;
    private IMetadataGenerators m_MetadataGenerators;
    private IMetadataCharacterSets m_MetadataCharacterSets;
    private IMetadataIndices m_MetadataIndices;
    private IMetadataProcedures m_MetadataProcedures;
    private IMetadataRelations m_MetadataRelations;
    private IMetadataRoles m_MetadataRoles;
    private IMetadataTriggers m_MetadataTriggers;
    private IMetadataUserPrivileges m_MetadataUserPrivileges;
    private IMetadataPackages m_MetadataPackages;

    public string ConnectionString { get; }
    public IDatabaseObjectFactory DatabaseObjectFactory { get; }
    public IReadOnlyCollection<IDatabaseObject> DatabaseObjects => m_DatabaseObjects;
    public IMetadataCollations MetadataCollations => m_MetadataCollations ??= GetSpecificDatabaseObject<IMetadataCollations>();
    public IMetadataConstraints MetadataConstraints => m_MetadataConstraints ??= GetSpecificDatabaseObject<IMetadataConstraints>();
    public IMetadataDatabase MetadataDatabase => m_MetadataDatabase ??= GetSpecificDatabaseObject<IMetadataDatabase>();
    public IMetadataDependencies MetadataDependencies => m_MetadataDependencies ??= GetSpecificDatabaseObject<IMetadataDependencies>();
    public IMetadataExceptions MetadataExceptions => m_MetadataExceptions ??= GetSpecificDatabaseObject<IMetadataExceptions>();
    public IMetadataFields MetadataFields => m_MetadataFields ??= GetSpecificDatabaseObject<IMetadataFields>();
    public IMetadataFunctions MetadataFunctions => m_MetadataFunctions ??= GetSpecificDatabaseObject<IMetadataFunctions>();
    public IMetadataGenerators MetadataGenerators => m_MetadataGenerators ??= GetSpecificDatabaseObject<IMetadataGenerators>();
    public IMetadataCharacterSets MetadataCharacterSets => m_MetadataCharacterSets ??= GetSpecificDatabaseObject<IMetadataCharacterSets>();
    public IMetadataIndices MetadataIndices => m_MetadataIndices ??= GetSpecificDatabaseObject<IMetadataIndices>();
    public IMetadataProcedures MetadataProcedures => m_MetadataProcedures ??= GetSpecificDatabaseObject<IMetadataProcedures>();
    public IMetadataRelations MetadataRelations => m_MetadataRelations ??= GetSpecificDatabaseObject<IMetadataRelations>();
    public IMetadataRoles MetadataRoles => m_MetadataRoles ??= GetSpecificDatabaseObject<IMetadataRoles>();
    public IMetadataTriggers MetadataTriggers => m_MetadataTriggers ??= GetSpecificDatabaseObject<IMetadataTriggers>();
    public IMetadataUserPrivileges MetadataUserPrivileges => m_MetadataUserPrivileges ??= GetSpecificDatabaseObject<IMetadataUserPrivileges>();
    public IMetadataPackages MetadataPackages => m_MetadataPackages ??= GetSpecificDatabaseObject<IMetadataPackages>();
    public T GetSpecificDatabaseObject<T>() where T : IDatabaseObject
    {
        return DatabaseObjects.OfType<T>().SingleOrDefault();
    }

    public void Initialize()
    {
        if (m_DatabaseObjects == null)
        {
            m_DatabaseObjects = new ReadOnlyCollection<IDatabaseObject>(DatabaseObjectFactory.ResolveAll(this));

            foreach (var databaseObject in m_DatabaseObjects)
            {
                databaseObject.Initialize();
            }

            foreach (var databaseObject in m_DatabaseObjects)
            {
                databaseObject.FinishInitialization();
            }
        }
    }
}
