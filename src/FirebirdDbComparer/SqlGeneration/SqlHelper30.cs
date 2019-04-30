using System;
using System.Collections.Generic;
using System.Text;

using FirebirdDbComparer.Compare;
using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.Elements;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Exceptions;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.SqlGeneration
{
    public class SqlHelper30 : SqlHelper25
    {
        public override TargetVersion TargetVersion => TargetVersion.Version30;

        public override DatabaseStringOrdinal[] Keywords => new DatabaseStringOrdinal[] { "!<", "!=", "!>", "(", ")", ",", "<", "<=", "<>", "=", ">", ">=", ":=", "ABS", "ABSOLUTE", "ACCENT", "ACOS", "ACOSH", "ACTION", "ACTIVE", "ADD", "ADMIN", "AFTER", "ALL", "ALTER", "ALWAYS", "AND", "ANY", "AS", "ASC", "ASCENDING", "ASCII_CHAR", "ASCII_VAL", "ASIN", "ASINH", "AT", "ATAN", "ATAN2", "ATANH", "AUTO", "AUTONOMOUS", "AVG", "BACKUP", "BEFORE", "BEGIN", "BETWEEN", "BIGINT", "BIN_AND", "BIN_NOT", "BIN_OR", "BIN_SHL", "BIN_SHR", "BIN_XOR", "BIT_LENGTH", "BLOB", "BLOCK", "BODY", "BOOLEAN", "BOTH", "BREAK", "BY", "CALLER", "CASCADE", "CASE", "CAST", "CEIL", "CEILING", "CHAR", "CHAR_LENGTH", "CHAR_TO_UUID", "CHARACTER", "CHARACTER_LENGTH", "CHECK", "CLOSE", "COALESCE", "COLLATE", "COLLATION", "COLUMN", "COMMENT", "COMMIT", "COMMITTED", "COMMON", "COMPUTED", "CONDITIONAL", "CONNECT", "CONSTRAINT", "CONTAINING", "CONTINUE", "CORR", "COS", "COSH", "COT", "COUNT", "COVAR_POP", "COVAR_SAMP", "CREATE", "CROSS", "CSTRING", "CURRENT", "CURRENT_CONNECTION", "CURRENT_DATE", "CURRENT_ROLE", "CURRENT_TIME", "CURRENT_TIMESTAMP", "CURRENT_TRANSACTION", "CURRENT_USER", "CURSOR", "DATABASE", "DATA", "DATE", "DATEADD", "DATEDIFF", "DAY", "DDL", "DEC", "DECIMAL", "DECLARE", "DECODE", "DECRYPT", "DEFAULT", "DELETE", "DELETING", "DENSE_RANK", "DESC", "DESCENDING", "DESCRIPTOR", "DETERMINISTIC", "DIFFERENCE", "DISCONNECT", "DISTINCT", "DO", "DOMAIN", "DOUBLE", "DROP", "ELSE", "ENCRYPT", "END", "ENGINE", "ENTRY_POINT", "ESCAPE", "EXCEPTION", "EXECUTE", "EXISTS", "EXIT", "EXP", "EXTERNAL", "EXTRACT", "FALSE", "FETCH", "FILE", "FILTER", "FIRST", "FIRST_VALUE", "FIRSTNAME", "FLOAT", "FLOOR", "FOR", "FOREIGN", "FREE_IT", "FROM", "FULL", "FUNCTION", "GDSCODE", "GENERATED", "GENERATOR", "GEN_ID", "GEN_UUID", "GLOBAL", "GRANT", "GRANTED", "GROUP", "HASH", "HAVING", "HOUR", "IDENTITY", "IF", "IGNORE", "IIF", "IN", "INACTIVE", "INCREMENT", "INDEX", "INNER", "INPUT_TYPE", "INSENSITIVE", "INSERT", "INSERTING", "INT", "INTEGER", "INTO", "IS", "ISOLATION", "JOIN", "KEY", "LAG", "LAST", "LAST_VALUE", "LASTNAME", "LEAD", "LEADING", "LEAVE", "LEFT", "LENGTH", "LEVEL", "LIKE", "LIMBO", "LINGER", "LIST", "LN", "LOCK", "LOG", "LOG10", "LONG", "LOWER", "LPAD", "MANUAL", "MAPPING", "MATCHED", "MATCHING", "MAX", "MAXVALUE", "MERGE", "MILLISECOND", "MIDDLENAME", "MIN", "MINUTE", "MINVALUE", "MOD", "MODULE_NAME", "MONTH", "NAME", "NAMES", "NATIONAL", "NATURAL", "NCHAR", "NEXT", "NO", "NOT", "NTH_VALUE", "NULLIF", "NULL", "NULLS", "NUMERIC", "OCTET_LENGTH", "OF", "OFFSET", "ON", "ONLY", "OPEN", "OPTION", "OR", "ORDER", "OS_NAME", "OUTER", "OUTPUT_TYPE", "OVER", "OVERFLOW", "OVERLAY", "PACKAGE", "PAD", "PAGE", "PAGES", "PAGE_SIZE", "PARAMETER", "PARTITION", "PASSWORD", "PI", "PLACING", "PLAN", "PLUGIN", "POSITION", "POST_EVENT", "POWER", "PRECISION", "PRESERVE", "PRIMARY", "PRIOR", "PRIVILEGES", "PROCEDURE", "PROTECTED", "RAND", "RANK", "RDB$DB_KEY", "RDB$GET_CONTEXT", "RDB$RECORD_VERSION", "RDB$SET_CONTEXT", "READ", "REAL", "RECORD_VERSION", "RECREATE", "RECURSIVE", "REFERENCES", "REGR_AVGX", "REGR_AVGY", "REGR_COUNT", "REGR_INTERCEPT", "REGR_R2", "REGR_SLOPE", "REGR_SXX", "REGR_SXY", "REGR_SYY", "RELATIVE", "RELEASE", "REPLACE", "REQUESTS", "RESERV", "RESERVING", "RESTART", "RESTRICT", "RETAIN", "RETURN", "RETURNING", "RETURNING_VALUES", "RETURNS", "REVERSE", "REVOKE", "RIGHT", "ROLE", "ROLLBACK", "ROUND", "ROW", "ROW_COUNT", "ROW_NUMBER", "ROWS", "RPAD", "SAVEPOINT", "SCALAR_ARRAY", "SCHEMA", "SCROLL", "SECOND", "SEGMENT", "SELECT", "SENSITIVE", "SEQUENCE", "SERVERWIDE", "SET", "SHADOW", "SHARED", "SIGN", "SIMILAR", "SIN", "SINGULAR", "SINH", "SIZE", "SKIP", "SMALLINT", "SNAPSHOT", "SOME", "SORT", "SOURCE", "SPACE", "SQLCODE", "SQLSTATE", "SQRT", "STABILITY", "START", "STARTING", "STARTS", "STATEMENT", "STATISTICS", "STDDEV_POP", "STDDEV_SAMP", "SUBSTRING", "SUB_TYPE", "SUM", "SUSPEND", "TABLE", "TAGS", "TAN", "TANH", "TEMPORARY", "THEN", "TIME", "TIMESTAMP", "TIMEOUT", "TO", "TRAILING", "TRANSACTION", "TRIGGER", "TRIM", "TRUE", "TRUNC", "TRUSTED", "TWO_PHASE", "TYPE", "UNCOMMITTED", "UNDO", "UNION", "UNIQUE", "UNKNOWN", "UPDATE", "UPDATING", "UPPER", "USAGE", "USER", "USING", "UUID_TO_CHAR", "VALUE", "VALUES", "VAR_POP", "VAR_SAMP", "VARCHAR", "VARIABLE", "VARYING", "VIEW", "WAIT", "WEEK", "WEEKDAY", "WHEN", "WHERE", "WHILE", "WITH", "WORK", "WRITE", "YEAR", "YEARDAY", "^<", "^=", "^>", "||", "~<", "~=", "~>" };

        public override IEnumerable<Command> HandleAlterCollation(Identifier fieldName, Identifier relationName, IHasCollation collation, IHasCollation otherCollation)
        {
            if (collation.CollationId != null && otherCollation.CollationId != collation.CollationId)
            {
                throw new NotSupportedOnFirebirdException($"Altering collation on the field is not supported ({relationName}.{fieldName}).");
            }

            yield break;
        }

        public override IEnumerable<Command> HandleAlterNullable(Identifier fieldName, Identifier relationName, IHasNullable nullable, IHasNullable otherNullable)
        {
            if (otherNullable.Nullable != nullable.Nullable)
            {
                var builder = new StringBuilder();
                builder.Append("ALTER ");
                builder.Append(relationName != null ? $"TABLE {relationName} ALTER {fieldName}" : $"DOMAIN {fieldName}");
                builder.Append($" {(nullable.Nullable ? "DROP" : "SET")} NOT NULL");
                yield return new Command().Append(builder.ToString());
            }
        }

        public override string GetDataType(FunctionArgument functionArgument, IDictionary<int, CharacterSet> characterSets, int defaultCharacterSetId)
        {
            if (functionArgument.Function.IsLegacy)
            {
                return base.GetDataType(functionArgument, characterSets, defaultCharacterSetId);
            }

            var builder = new StringBuilder();
            if (functionArgument.RelationName != null && functionArgument.FieldName != null)
            {
                builder.Append($"TYPE OF COLUMN {functionArgument.RelationName.AsSqlIndentifier()}.{functionArgument.FieldName.AsSqlIndentifier()}");
            }
            else
            {
                switch (functionArgument.Field.MetadataFieldType)
                {
                    case MetadataFieldType.SYSTEM_GENERATED:
                        builder.Append(GetDataType(functionArgument.Field, characterSets, defaultCharacterSetId));
                        break;
                    case MetadataFieldType.DOMAIN:
                        if (functionArgument.ArgumentMechanismNewStyle == ProcedureParameterMechanism.TYPE_OF)
                        {
                            builder.Append("TYPE OF ");
                        }
                        builder.Append(functionArgument.FieldSource.AsSqlIndentifier());
                        break;
                    default:
                        throw new NotSupportedException($"Unknown field type: {functionArgument.Field.MetadataFieldType}.");
                }
            }

            return builder.ToString();
        }

        public override string GetDataType(IDataType dataType, IDictionary<int, CharacterSet> characterSets, int defaultCharacterSetId)
        {
            switch (dataType.FieldType)
            {
                case FieldType.BOOLEAN:
                    return "BOOLEAN";
                default:
                    return base.GetDataType(dataType, characterSets, defaultCharacterSetId);
            }
        }
    }
}
