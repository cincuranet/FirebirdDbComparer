using System;
using System.Collections.Generic;
using FirebirdDbComparer.Common;
using FirebirdDbComparer.Compare;
using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.SqlGeneration;

public class SqlHelper40 : SqlHelper30
{
    public override TargetVersion TargetVersion => TargetVersion.Version40;

    public override DatabaseStringOrdinal[] Keywords => new DatabaseStringOrdinal[] { "!<", "!=", "!>", "(", ")", ",", "<", "<=", "<>", "=", ">", ">=", ":=", "ABS", "ABSOLUTE", "ACCENT", "ACOS", "ACOSH", "ACTION", "ACTIVE", "ADD", "ADMIN", "AFTER", "ALL", "ALTER", "ALWAYS", "AND", "ANY", "AS", "ASC", "ASCENDING", "ASCII_CHAR", "ASCII_VAL", "ASIN", "ASINH", "AT", "ATAN", "ATAN2", "ATANH", "AUTO", "AUTONOMOUS", "AVG", "BACKUP", "BASE64_DECODE", "BASE64_ENCODE", "BEFORE", "BEGIN", "BETWEEN", "BIGINT", "BIN_AND", "BIN_NOT", "BIN_OR", "BIN_SHL", "BIN_SHR", "BIN_XOR", "BINARY", "BIND", "BIT_LENGTH", "BLOB", "BLOCK", "BODY", "BOOLEAN", "BOTH", "BREAK", "BY", "CALLER", "CASCADE", "CASE", "CAST", "CEIL", "CEILING", "CHAR", "CHAR_LENGTH", "CHAR_TO_UUID", "CHARACTER", "CHARACTER_LENGTH", "CHECK", "CLEAR", "CLOSE", "COALESCE", "COLLATE", "COLLATION", "COLUMN", "COMMENT", "COMMIT", "COMMITTED", "COMMON", "COMPARE_DECFLOAT", "COMPUTED", "CONDITIONAL", "CONNECT", "CONNECTIONS", "CONSISTENCY", "CONSTRAINT", "CONTAINING", "CONTINUE", "CORR", "COS", "COSH", "COT", "COUNT", "COUNTER", "COVAR_POP", "COVAR_SAMP", "CREATE", "CROSS", "CRYPT_HASH", "CSTRING", "CTR_BIG_ENDIAN", "CTR_LENGTH", "CTR_LITTLE_ENDIAN", "CUME_DIST", "CURRENT", "CURRENT_CONNECTION", "CURRENT_DATE", "CURRENT_ROLE", "CURRENT_TIME", "CURRENT_TIMESTAMP", "CURRENT_TRANSACTION", "CURRENT_USER", "CURSOR", "DATABASE", "DATA", "DATE", "DATEADD", "DATEDIFF", "DAY", "DDL", "DEBUG", "DEC", "DECFLOAT", "DECIMAL", "DECLARE", "DECODE", "DECRYPT", "DEFAULT", "DEFINER", "DELETE", "DELETING", "DENSE_RANK", "DESC", "DESCENDING", "DESCRIPTOR", "DETERMINISTIC", "DIFFERENCE", "DISABLE", "DISCONNECT", "DISTINCT", "DO", "DOMAIN", "DOUBLE", "DROP", "ELSE", "ENABLE", "ENCRYPT", "END", "ENGINE", "ENTRY_POINT", "ESCAPE", "EXCEPTION", "EXCESS", "EXCLUDE", "EXECUTE", "EXISTS", "EXIT", "EXP", "EXTENDED", "EXTERNAL", "EXTRACT", "FALSE", "FETCH", "FILE", "FILTER", "FIRST", "FIRST_DAY", "FIRST_VALUE", "FIRSTNAME", "FLOAT", "FLOOR", "FOLLOWING", "FOR", "FOREIGN", "FREE_IT", "FROM", "FULL", "FUNCTION", "GDSCODE", "GENERATED", "GENERATOR", "GEN_ID", "GEN_UUID", "GLOBAL", "GRANT", "GRANTED", "GROUP", "HASH", "HAVING", "HEX_DECODE", "HEX_ENCODE", "HOUR", "IDENTITY", "IDLE", "IF", "IGNORE", "IIF", "IN", "INACTIVE", "INCLUDE", "INCREMENT", "INDEX", "INNER", "INPUT_TYPE", "INSENSITIVE", "INSERT", "INSERTING", "INT", "INT128", "INTEGER", "INTO", "INVOKER", "IS", "ISOLATION", "IV", "JOIN", "KEY", "LAG", "LAST", "LAST_DAY", "LAST_VALUE", "LASTNAME", "LEAD", "LEADING", "LEAVE", "LEFT", "LEGACY", "LENGTH", "LEVEL", "LIFETIME", "LIKE", "LIMBO", "LINGER", "LIST", "LN", "LATERAL", "LOCAL", "LOCALTIME", "LOCALTIMESTAMP", "LOCK", "LOG", "LOG10", "LONG", "LOWER", "LPAD", "LPARAM", "MAKE_DBKEY", "MANUAL", "MAPPING", "MATCHED", "MATCHING", "MAX", "MAXVALUE", "MERGE", "MESSAGE", "MILLISECOND", "MIDDLENAME", "MIN", "MINUTE", "MINVALUE", "MOD", "MODE", "MODULE_NAME", "MONTH", "NAME", "NAMES", "NATIONAL", "NATIVE", "NATURAL", "NCHAR", "NEXT", "NO", "NORMALIZE_DECFLOAT", "NOT", "NTH_VALUE", "NTILE", "NULLIF", "NULL", "NULLS", "NUMBER", "NUMERIC", "OCTET_LENGTH", "OF", "OFFSET", "OLDEST", "ON", "ONLY", "OPEN", "OPTION", "OR", "ORDER", "OS_NAME", "OTHERS", "OUTER", "OUTPUT_TYPE", "OVER", "OVERFLOW", "OVERLAY", "OVERRIDING", "PACKAGE", "PAD", "PAGE", "PAGES", "PAGE_SIZE", "PARAMETER", "PARTITION", "PASSWORD", "PERCENT_RANK", "PI", "PKCS_1_5", "PLACING", "PLAN", "PLUGIN", "POOL", "POSITION", "POST_EVENT", "POWER", "PRECEDING", "PRECISION", "PRESERVE", "PRIMARY", "PRIOR", "PRIVILEGE", "PRIVILEGES", "PROCEDURE", "PROTECTED", "PUBLICATION", "QUANTIZE", "RAND", "RANGE", "RANK", "RDB$DB_KEY", "RDB$ERROR", "RDB$GET_CONTEXT", "RDB$GET_TRANSACTION_CN", "RDB$RECORD_VERSION", "RDB$ROLE_IN_USE", "RDB$SET_CONTEXT", "RDB$SYSTEM_PRIVILEGE", "READ", "REAL", "RECORD_VERSION", "RECREATE", "RECURSIVE", "REFERENCES", "REGR_AVGX", "REGR_AVGY", "REGR_COUNT", "REGR_INTERCEPT", "REGR_R2", "REGR_SLOPE", "REGR_SXX", "REGR_SXY", "REGR_SYY", "RELATIVE", "RELEASE", "REPLACE", "REQUESTS", "RESERV", "RESERVING", "RESET", "RESETTING", "RESTART", "RESTRICT", "RETAIN", "RETURN", "RETURNING", "RETURNING_VALUES", "RETURNS", "REVERSE", "REVOKE", "RIGHT", "ROLE", "ROLLBACK", "ROUND", "ROW", "ROW_COUNT", "ROW_NUMBER", "ROWS", "RPAD", "RSA_DECRYPT", "RSA_ENCRYPT", "RSA_PRIVATE", "RSA_PUBLIC", "RSA_SIGN_HASH", "RSA_VERIFY_HASH", "SALT_LENGTH", "SAVEPOINT", "SCALAR_ARRAY", "SCHEMA", "SCROLL", "SECOND", "SECURITY", "SEGMENT", "SELECT", "SENSITIVE", "SEQUENCE", "SERVERWIDE", "SESSION", "SET", "SHADOW", "SHARED", "SIGN", "SIGNATURE", "SIMILAR", "SIN", "SINGULAR", "SINH", "SIZE", "SKIP", "SMALLINT", "SNAPSHOT", "SOME", "SORT", "SOURCE", "SPACE", "SQL", "SQLCODE", "SQLSTATE", "SQRT", "STABILITY", "START", "STARTING", "STARTS", "STATEMENT", "STATISTICS", "STDDEV_POP", "STDDEV_SAMP", "SUBSTRING", "SUB_TYPE", "SUM", "SUSPEND", "SYSTEM", "TABLE", "TAGS", "TAN", "TANH", "TEMPORARY", "THEN", "TIES", "TIME", "TIMESTAMP", "TIMEOUT", "TIMEZONE_HOUR", "TIMEZONE_MINUTE", "TO", "TOTALORDER", "TRAILING", "TRANSACTION", "TRAPS", "TRIGGER", "TRIM", "TRUE", "TRUNC", "TRUSTED", "TWO_PHASE", "TYPE", "UNBOUNDED", "UNCOMMITTED", "UNDO", "UNION", "UNIQUE", "UNKNOWN", "UPDATE", "UPDATING", "UPPER", "USAGE", "USER", "USING", "UUID_TO_CHAR", "VALUE", "VALUES", "VAR_POP", "VAR_SAMP", "VARBINARY", "VARCHAR", "VARIABLE", "VARYING", "VIEW", "WAIT", "WEEK", "WEEKDAY", "WHEN", "WHERE", "WHILE", "WINDOW", "WITH", "WITHOUT", "WORK", "WRITE", "YEAR", "YEARDAY", "ZONE", "^<", "^=", "^>", "||", "~<", "~=", "~>" };

    public override string GetDataType(IDataType dataType, IDictionary<int, CharacterSet> characterSets, int defaultCharacterSetId)
    {
        return dataType.FieldType switch
        {
            FieldType.DecFloat16 => "DECFLOAT(16)",
            FieldType.DecFloat34 => "DECFLOAT(34)",
            FieldType.Int128 => ScalableNumber("INT128", dataType),
            FieldType.TimeWithTimeZone => "TIME WITH TIME ZONE",
            FieldType.TimestampWithTimeZone => "TIMESTAMP WITH TIME ZONE",
            _ => base.GetDataType(dataType, characterSets, defaultCharacterSetId),
        };
    }

    /// <summary>
    /// for more information see \src\jrd\obj.h
    /// </summary>
    public override string ObjectTypeString(int objectType)
    {
        return objectType switch
        {
            0 => "TABLE",
            1 => "VIEW",
            2 => "TRIGGER",
            5 => "PROCEDURE",
            7 => "EXCEPTION",
            8 => "USER",
            9 => "DOMAIN",
            10 => "INDEX",
            11 => "CHARACTER SET",
            13 => "ROLE",
            14 => "SEQUENCE",
            15 => "FUNCTION",
            17 => "COLLATION",
            18 => "PACKAGE",
            20 => "SYSTEM PRIVILEGE",
            21 => "DATABASE",
            22 => "TABLE",
            23 => "VIEW",
            24 => "PROCEDURE",
            25 => "FUNCTION",
            26 => "PACKAGE",
            27 => "SEQUENCE",
            28 => "DOMAIN",
            29 => "EXCEPTION",
            30 => "ROLE",
            31 => "CHARACTER SET",
            32 => "COLLATION",
            33 => "FILTER",
            _ => throw new ArgumentOutOfRangeException($"Wrong object type: {objectType}."),
        };
    }

    public override bool ObjectTypeIsSystemPrivilege(int objectType) => objectType == 20;

    public override string SystemPrivilegeString(int systemPrivilege)
    {
        return systemPrivilege switch
        {
            1 => "USER_MANAGEMENT",
            2 => "READ_RAW_PAGES",
            3 => "CREATE_USER_TYPES",
            4 => "USE_NBACKUP_UTILITY",
            5 => "CHANGE_SHUTDOWN_MODE",
            6 => "TRACE_ANY_ATTACHMENT",
            7 => "MONITOR_ANY_ATTACHMENT",
            8 => "ACCESS_SHUTDOWN_DATABASE",
            9 => "CREATE_DATABASE",
            10 => "DROP_DATABASE",
            11 => "USE_GBAK_UTILITY",
            12 => "USE_GSTAT_UTILITY",
            13 => "USE_GFIX_UTILITY",
            14 => "IGNORE_DB_TRIGGERS",
            15 => "CHANGE_HEADER_SETTINGS",
            16 => "SELECT_ANY_OBJECT_IN_DATABASE",
            17 => "ACCESS_ANY_OBJECT_IN_DATABASE",
            18 => "MODIFY_ANY_OBJECT_IN_DATABASE",
            19 => "CHANGE_MAPPING_RULES",
            20 => "USE_GRANTED_BY_CLAUSE",
            21 => "GRANT_REVOKE_ON_ANY_OBJECT",
            22 => "GRANT_REVOKE_ANY_DDL_RIGHT",
            23 => "CREATE_PRIVILEGED_ROLES",
            24 => "GET_DBCRYPT_INFO",
            25 => "MODIFY_EXT_CONN_POOL",
            26 => "REPLICATE_INTO_DATABASE",
            _ => throw new ArgumentOutOfRangeException($"Unknown privilege: {systemPrivilege}."),
        };
    }

    public override IEnumerable<Command> HandleAlterIdentity(RelationField field, RelationField otherField)
    {
        if (field.IdentityType == null && otherField.IdentityType != null)
        {
            yield return new Command().Append($"ALTER TABLE {field.RelationName.AsSqlIndentifier()} ALTER {field.FieldName.AsSqlIndentifier()} DROP IDENTITY");
        }
        else if (field.IdentityType != null && otherField.IdentityType != null && field.IdentityType != otherField.IdentityType)
        {
            var command = new Command().Append($"ALTER TABLE {field.RelationName.AsSqlIndentifier()} ALTER {field.FieldName.AsSqlIndentifier()} SET GENERATED {field.IdentityType.ToDescription()}");
            if (field.Generator.GeneratorIncrement != otherField.Generator.GeneratorIncrement)
            {
                command.Append($" SET INCREMENT BY {field.Generator.GeneratorIncrement}");
            }
            yield return command;
        }
        else if (field.IdentityType != null && otherField.IdentityType != null && field.IdentityType == otherField.IdentityType && field.Generator.GeneratorIncrement != otherField.Generator.GeneratorIncrement)
        {
            yield return new Command().Append($"ALTER TABLE {field.RelationName.AsSqlIndentifier()} ALTER {field.FieldName.AsSqlIndentifier()} SET INCREMENT BY {field.Generator.GeneratorIncrement}");
        }
        else
        {
            foreach (var item in base.HandleAlterIdentity(field, otherField))
                yield return item;
        }
    }

    public override string SqlSecurityString(bool? sqlSecurity)
    {
        return sqlSecurity switch
        {
            true => "DEFINER",
            false => "INVOKER",
            null => null,
        };
    }
}
