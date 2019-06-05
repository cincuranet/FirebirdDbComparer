using System.ComponentModel;

namespace FirebirdDbComparer.DatabaseObjects
{
    /// <summary>
    /// for more information see \src\jrd\obj.h
    /// </summary>
    public enum ObjectType
    {
        [Description("TABLE")]
        Relation = 0,
        View = 1,
        Trigger = 2,
        ComputedField = 3,
        Validation = 4,
        Procedure = 5,
        ExpressionIndex = 6,
        Exception = 7,
        User = 8,

        [Description("DOMAIN")]
        Field = 9,
        Index = 10,

        [Description("CHARACTER SET")]
        CharacterSet = 11,
        UserGroup = 12,
        Role = 13,

        [Description("SEQUENCE")]
        Generator = 14,

        [Description("FUNCTION")]
        UDF = 15,
        BlobFilter = 16,
        Collation = 17,
        Package = 18,
        PackageBody = 19,

        [Description("DATABASE")]
        DDLDatabase = 20,
        
        [Description("TABLE")]
        DDLRelations = 21,

        [Description("VIEW")]
        DDLViews = 22,

        [Description("PROCEDURE")]
        DDLProcedure = 23,

        [Description("FUNCTION")]
        DDLFunctions = 24,

        [Description("PACKAGE")]
        DDLPackages = 25,

        [Description("SEQUENCE")]
        DDLGenerators = 26,

        [Description("DOMAIN")]
        DDLDomains = 27,

        [Description("EXCEPTION")]
        DDLExceptions = 28,

        [Description("ROLE")]
        DDLRoles = 29,

        [Description("CHARACTER SET")]
        DDLCharacterSet = 30,

        [Description("COLLATION")]
        DDLCollations = 31,

        [Description("FILTER")]
        DDLFilters = 32,
    }
}
