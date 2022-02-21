using System;
using System.Collections.Generic;

using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects;

internal class Metadata : IMetadata
{
    public Metadata(string connectionString, Func<Metadata, IReadOnlyCollection<IDatabaseObject>> databaseObjectFactory)
    {
        ConnectionString = connectionString;
        m_DatabaseObjectFactory = databaseObjectFactory ?? throw new ArgumentNullException(nameof(databaseObjectFactory));
    }

    private readonly Func<Metadata, IReadOnlyCollection<IDatabaseObject>> m_DatabaseObjectFactory;

    public string ConnectionString { get; }
    public IReadOnlyCollection<IDatabaseObject> DatabaseObjects { get; private set; }
    public IMetadataCollations MetadataCollations { get; private set; }
    public IMetadataConstraints MetadataConstraints { get; private set; }
    public IMetadataDatabase MetadataDatabase { get; private set; }
    public IMetadataDependencies MetadataDependencies { get; private set; }
    public IMetadataExceptions MetadataExceptions { get; private set; }
    public IMetadataFields MetadataFields { get; private set; }
    public IMetadataFunctions MetadataFunctions { get; private set; }
    public IMetadataGenerators MetadataGenerators { get; private set; }
    public IMetadataCharacterSets MetadataCharacterSets { get; private set; }
    public IMetadataIndices MetadataIndices { get; private set; }
    public IMetadataProcedures MetadataProcedures { get; private set; }
    public IMetadataRelations MetadataRelations { get; private set; }
    public IMetadataRoles MetadataRoles { get; private set; }
    public IMetadataTriggers MetadataTriggers { get; private set; }
    public IMetadataUserPrivileges MetadataUserPrivileges { get; private set; }
    public IMetadataPackages MetadataPackages { get; private set; }

    public void Initialize()
    {
        if (DatabaseObjects == null)
        {
            DatabaseObjects = m_DatabaseObjectFactory.Invoke(this);

            foreach (var databaseObject in DatabaseObjects)
            {
                switch (databaseObject)
                {
                    case IMetadataCollations metadataCollations:
                        MetadataCollations = metadataCollations;
                        break;
                    case IMetadataConstraints metadataConstraints:
                        MetadataConstraints = metadataConstraints;
                        break;
                    case IMetadataDatabase metadataDatabase:
                        MetadataDatabase = metadataDatabase;
                        break;
                    case IMetadataDependencies metadataDependencies:
                        MetadataDependencies = metadataDependencies;
                        break;
                    case IMetadataExceptions metadataExceptions:
                        MetadataExceptions = metadataExceptions;
                        break;
                    case IMetadataFields metadataFields:
                        MetadataFields = metadataFields;
                        break;
                    case IMetadataFunctions metadataFunctions:
                        MetadataFunctions = metadataFunctions;
                        break;
                    case IMetadataGenerators metadataGenerators:
                        MetadataGenerators = metadataGenerators;
                        break;
                    case IMetadataCharacterSets metadataCharacterSets:
                        MetadataCharacterSets = metadataCharacterSets;
                        break;
                    case IMetadataIndices metadataIndices:
                        MetadataIndices = metadataIndices;
                        break;
                    case IMetadataProcedures metadataProcedures:
                        MetadataProcedures = metadataProcedures;
                        break;
                    case IMetadataRelations metadataRelations:
                        MetadataRelations = metadataRelations;
                        break;
                    case IMetadataRoles metadataRoles:
                        MetadataRoles = metadataRoles;
                        break;
                    case IMetadataTriggers metadataTriggers:
                        MetadataTriggers = metadataTriggers;
                        break;
                    case IMetadataUserPrivileges metadataUserPrivileges:
                        MetadataUserPrivileges = metadataUserPrivileges;
                        break;
                    case IMetadataPackages metadataPackages:
                        MetadataPackages = metadataPackages;
                        break;
                }
            }

            foreach (var databaseObject in DatabaseObjects)
            {
                databaseObject.Initialize();
            }

            foreach (var databaseObject in DatabaseObjects)
            {
                databaseObject.FinishInitialization();
            }
        }
    }
}
