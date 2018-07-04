using System.ComponentModel;
using System.ComponentModel.Design;

namespace FirebirdDbComparer.DatabaseObjects
{
    /// <summary>
    /// for more information see \src\jrd\obj.h
    /// </summary>
    public enum ObjectType
    {
        [Description("TABLE")]
        RELATION = 0,
        VIEW = 1,
        TRIGGER = 2,
        COMPUTED_FIELD = 3,
        VALIDATION = 4,
        PROCEDURE = 5,
        EXPRESSION_INDEX = 6,
        EXCEPTION = 7,
        USER = 8,

        [Description("DOMAIN")]
        FIELD = 9,
        INDEX = 10,

        [Description("CHARACTER SET")]
        CHARACTER_SET = 11,
        USER_GROUP = 12,
        ROLE = 13,

        [Description("SEQUENCE")]
        GENERATOR = 14,

        [Description("FUNCTION")]
        UDF = 15,
        BLOB_FILTER = 16,
        COLLATION = 17,
        PACKAGE = 18,
        PACKAGE_BODY = 19,

        [Description("DATABASE")]
        DDL_DATABASE = 20,
        
        [Description("TABLE")]
        DDL_RELATIONS = 21,

        [Description("VIEW")]
        DDL_VIEWS = 22,

        [Description("PROCEDURE")]
        DDL_PROCEDURE = 23,

        [Description("FUNCTION")]
        DDL_FUNCTIONS = 24,

        [Description("PACKAGE")]
        DDL_PACKAGES = 25,

        [Description("SEQUENCE")]
        DDL_GENERATORS = 26,

        [Description("DOMAIN")]
        DDL_DOMAINS = 27,

        [Description("EXCEPTION")]
        DDL_EXCEPTIONS = 28,

        [Description("ROLE")]
        DDL_ROLES = 29,

        [Description("CHARACTER SET")]
        DDL_CHARACTER_SET = 30,

        [Description("COLLATION")]
        DDL_COLLATIONS = 31,

        [Description("FILTER")]
        DDL_FILTERS = 32
    }
}
