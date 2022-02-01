using System.ComponentModel;

namespace FirebirdDbComparer.DatabaseObjects;

public enum TriggerEventType
{
    [Description("INSERT")]
    Insert,

    [Description("UPDATE")]
    Update,

    [Description("DELETE")]
    Delete,

    [Description("CONNECT")]
    Connect,

    [Description("DISCONNECT")]
    Disconnect,

    [Description("TRANSACTION START")]
    TransactionStart,

    [Description("TRANSACTION COMMIT")]
    TransactionCommit,

    [Description("TRANSACTION ROLLBACK")]
    TransactionRollback,

    [Description("ANY DDL STATEMENT")]
    Any,

    [Description("CREATE TABLE")]
    CreateTable,

    [Description("ALTER TABLE")]
    AlterTable,

    [Description("DROP TABLE")]
    DropTable,

    [Description("CREATE PROCEDURE")]
    CreateProcedure,

    [Description("ALTER PROCEDURE")]
    AlterProcedure,

    [Description("DROP PROCEDURE")]
    DropProcedure,

    [Description("CREATE FUNCTION")]
    CreateFunction,

    [Description("ALTER FUNCTION")]
    AlterFunction,

    [Description("DROP FUNCTION")]
    DropFunction,

    [Description("CREATE TRIGGER")]
    CreateTrigger,

    [Description("ALTER TRIGGER")]
    AlterTrigger,

    [Description("DROP TRIGGER")]
    DropTrigger,

    [Description("CREATE EXCEPTION")]
    CreateException,

    [Description("ALTER EXCEPTION")]
    AlterException,

    [Description("DROP EXCEPTION")]
    DropException,

    [Description("CREATE VIEW")]
    CreateView,

    [Description("ALTER VIEW")]
    AlterView,

    [Description("DROP VIEW")]
    DropView,

    [Description("CREATE DOMAIN")]
    CreateDomain,

    [Description("ALTER DOMAIN")]
    AlterDomain,

    [Description("DROP DOMAIN")]
    DropDomain,

    [Description("CREATE ROLE")]
    CreateRole,

    [Description("ALTER ROLE")]
    AlterRole,

    [Description("DROP ROLE")]
    DropRole,

    [Description("CREATE INDEX")]
    CreateIndex,

    [Description("ALTER INDEX")]
    AlterIndex,

    [Description("DROP INDEX")]
    DropIndex,

    [Description("CREATE SEQUENCE")]
    CreateSequence,

    [Description("ALTER SEQUENCE")]
    AlterSequence,

    [Description("DROP SEQUENCE")]
    DropSequence,

    [Description("CREATE USER")]
    CreateUser,

    [Description("ALTER USER")]
    AlterUser,

    [Description("DROP USER")]
    DropUser,

    [Description("CREATE COLLATION")]
    CreateCollation,

    [Description("DROP COLLATION")]
    DropCollation,

    [Description("ALTER CHARACTER SET")]
    AlterCharacterSet,

    [Description("CREATE PACKAGE")]
    CreatePackage,

    [Description("ALTER PACKAGE")]
    AlterPackage,

    [Description("DROP PACKAGE")]
    DropPackage,

    [Description("CREATE PACKAGE BODY")]
    CreatePackageBody,

    [Description("DROP PACKAGE BODY")]
    DropPackageBody,

    [Description("CREATE MAPPING")]
    CreateMapping,

    [Description("ALTER MAPPING")]
    AlterMapping,

    [Description("DROP MAPPING")]
    DropMapping,
}
