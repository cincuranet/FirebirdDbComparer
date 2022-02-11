using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using FirebirdDbComparer.Compare;
using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.Elements;
using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.Exceptions;
using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.SqlGeneration;

public class SqlHelper25 : ISqlHelper
{
    public virtual TargetVersion TargetVersion => TargetVersion.Version25;

    public virtual string Terminator => ";";
    public virtual string AlternativeTerminator => "^";

    public virtual DatabaseStringOrdinal[] Keywords => new DatabaseStringOrdinal[] { "!<", "!=", "!>", "(", ")", ",", "<", "<=", "<>", "=", ">", ">=", ":=", "ABS", "ACCENT", "ACOS", "ACTION", "ACTIVE", "ADD", "ADMIN", "AFTER", "ALL", "ALTER", "ALWAYS", "AND", "ANY", "AS", "ASC", "ASCENDING", "ASCII_CHAR", "ASCII_VAL", "ASIN", "AT", "ATAN", "ATAN2", "AUTO", "AUTONOMOUS", "AVG", "BACKUP", "BEFORE", "BEGIN", "BETWEEN", "BIGINT", "BIN_AND", "BIN_NOT", "BIN_OR", "BIN_SHL", "BIN_SHR", "BIN_XOR", "BIT_LENGTH", "BLOB", "BLOCK", "BOTH", "BREAK", "BY", "CALLER", "CASCADE", "CASE", "CAST", "CEIL", "CEILING", "CHAR", "CHAR_LENGTH", "CHAR_TO_UUID", "CHARACTER", "CHARACTER_LENGTH", "CHECK", "CLOSE", "COALESCE", "COLLATE", "COLLATION", "COLUMN", "COMMENT", "COMMIT", "COMMITTED", "COMMON", "COMPUTED", "CONDITIONAL", "CONNECT", "CONSTRAINT", "CONTAINING", "COS", "COSH", "COT", "COUNT", "CREATE", "CROSS", "CSTRING", "CURRENT", "CURRENT_CONNECTION", "CURRENT_DATE", "CURRENT_ROLE", "CURRENT_TIME", "CURRENT_TIMESTAMP", "CURRENT_TRANSACTION", "CURRENT_USER", "CURSOR", "DATABASE", "DATA", "DATE", "DATEADD", "DATEDIFF", "DAY", "DEC", "DECIMAL", "DECLARE", "DECODE", "DEFAULT", "DELETE", "DELETING", "DESC", "DESCENDING", "DESCRIPTOR", "DIFFERENCE", "DISCONNECT", "DISTINCT", "DO", "DOMAIN", "DOUBLE", "DROP", "ELSE", "END", "ENTRY_POINT", "ESCAPE", "EXCEPTION", "EXECUTE", "EXISTS", "EXIT", "EXP", "EXTERNAL", "EXTRACT", "FETCH", "FILE", "FILTER", "FIRST", "FIRSTNAME", "FLOAT", "FLOOR", "FOR", "FOREIGN", "FREE_IT", "FROM", "FULL", "FUNCTION", "GDSCODE", "GENERATED", "GENERATOR", "GEN_ID", "GEN_UUID", "GLOBAL", "GRANT", "GRANTED", "GROUP", "HASH", "HAVING", "HOUR", "IF", "IGNORE", "IIF", "IN", "INACTIVE", "INDEX", "INNER", "INPUT_TYPE", "INSENSITIVE", "INSERT", "INSERTING", "INT", "INTEGER", "INTO", "IS", "ISOLATION", "JOIN", "KEY", "LAST", "LASTNAME", "LEADING", "LEAVE", "LEFT", "LENGTH", "LEVEL", "LIKE", "LIMBO", "LIST", "LN", "LOCK", "LOG", "LOG10", "LONG", "LOWER", "LPAD", "MANUAL", "MAPPING", "MATCHED", "MATCHING", "MAX", "MAXVALUE", "MAXIMUM_SEGMENT", "MERGE", "MILLISECOND", "MIDDLENAME", "MIN", "MINUTE", "MINVALUE", "MOD", "MODULE_NAME", "MONTH", "NAMES", "NATIONAL", "NATURAL", "NCHAR", "NEXT", "NO", "NOT", "NULLIF", "NULL", "NULLS", "NUMERIC", "OCTET_LENGTH", "OF", "ON", "ONLY", "OPEN", "OPTION", "OR", "ORDER", "OS_NAME", "OUTER", "OUTPUT_TYPE", "OVERFLOW", "OVERLAY", "PAD", "PAGE", "PAGES", "PAGE_SIZE", "PARAMETER", "PASSWORD", "PI", "PLACING", "PLAN", "POSITION", "POST_EVENT", "POWER", "PRECISION", "PRESERVE", "PRIMARY", "PRIVILEGES", "PROCEDURE", "PROTECTED", "RAND", "RDB$DB_KEY", "READ", "REAL", "RECORD_VERSION", "RECREATE", "RECURSIVE", "REFERENCES", "RELEASE", "REPLACE", "REQUESTS", "RESERV", "RESERVING", "RESTART", "RESTRICT", "RETAIN", "RETURNING", "RETURNING_VALUES", "RETURNS", "REVERSE", "REVOKE", "RIGHT", "ROLE", "ROLLBACK", "ROUND", "ROW_COUNT", "ROWS", "RPAD", "SAVEPOINT", "SCALAR_ARRAY", "SCHEMA", "SECOND", "SEGMENT", "SELECT", "SENSITIVE", "SEQUENCE", "SET", "SHADOW", "SHARED", "SIGN", "SIMILAR", "SIN", "SINGULAR", "SINH", "SIZE", "SKIP", "SMALLINT", "SNAPSHOT", "SOME", "SORT", "SOURCE", "SPACE", "SQLCODE", "SQLSTATE", "SQRT", "STABILITY", "START", "STARTING", "STARTS", "STATEMENT", "STATISTICS", "SUBSTRING", "SUB_TYPE", "SUM", "SUSPEND", "TABLE", "TAN", "TANH", "TEMPORARY", "THEN", "TIME", "TIMESTAMP", "TIMEOUT", "TO", "TRAILING", "TRANSACTION", "TRIGGER", "TRIM", "TRUNC", "TWO_PHASE", "TYPE", "UNCOMMITTED", "UNDO", "UNION", "UNIQUE", "UPDATE", "UPDATING", "UPPER", "USER", "USING", "UUID_TO_CHAR", "VALUE", "VALUES", "VARCHAR", "VARIABLE", "VARYING", "VIEW", "WAIT", "WEEK", "WEEKDAY", "WHEN", "WHERE", "WHILE", "WITH", "WORK", "WRITE", "YEAR", "YEARDAY", "^<", "^=", "^>", "||", "~<", "~=", "~>" };

    public virtual string DoubleSingleQuotes(string value)
    {
        return value?.Replace("'", "''");
    }

    public virtual string DoubleSingleQuotes(DatabaseStringOrdinal value)
    {
        return DoubleSingleQuotes(value?.ToString());
    }

    public virtual string QuoteIdentifierIfNeeded(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }
        var needsQuoting =
            !value.All(c => c >= 'A' && c <= 'Z' || c >= '0' && c <= '9' || c == '_' || c == '$')
            || !(value[0] >= 'A' && value[0] <= 'Z')
            || Keywords.Contains(value);
        return needsQuoting
                   ? $"\"{value.Replace("\"", "\"\"")}\""
                   : value;
    }

    public virtual string QuoteIdentifierIfNeeded(DatabaseStringOrdinal value)
    {
        return QuoteIdentifierIfNeeded(value?.ToString());
    }

    public virtual string CreateComment(string objectTypeName, IEnumerable<Identifier> objectNames, DatabaseStringOrdinal description, DatabaseStringOrdinal otherDescription)
    {
        var objectName = string.Join(".", objectNames.Select(x => x.AsSqlIndentifier()));
        var commentStart = $"COMMENT ON {objectTypeName} {objectName}";
        if (description == null && otherDescription != null)
        {
            return $"{commentStart} IS NULL";
        }
        if (description != null && description != otherDescription)
        {
            return $"{commentStart} IS '{DoubleSingleQuotes(description)}'";
        }
        return null;
    }

    public virtual string GetDataType(IDataType dataType, IDictionary<int, CharacterSet> characterSets, int defaultCharacterSetId)
    {
        switch (dataType.FieldType)
        {
            case FieldType.Short:
                return ScalableNumber("SMALLINT", dataType);
            case FieldType.Long:
                return ScalableNumber("INT", dataType);
            case FieldType.Float:
                return "FLOAT";
            case FieldType.Date:
                return "DATE";
            case FieldType.Time:
                return "TIME";
            case FieldType.Int64:
                return ScalableNumber("BIGINT", dataType);
            case FieldType.Double:
                return "DOUBLE PRECISION";
            case FieldType.Timestamp:
                return "TIMESTAMP";
            case FieldType.Varying:
            case FieldType.Text:
                return VarcharChar(dataType, characterSets, defaultCharacterSetId);
            case FieldType.CString:
                return $"CSTRING({dataType.CharacterLength ?? dataType.FieldLength})";
            case FieldType.Blob:
                var segmentSize = dataType.SegmentSize != null
                    ? $" SEGMENT SIZE {dataType.SegmentSize}"
                    : string.Empty;
                return $"BLOB SUB_TYPE {dataType.FieldSubType}{segmentSize}";
            default:
                throw new ArgumentOutOfRangeException($"Unknown field type: {dataType.FieldType}.");
        }
    }

    public virtual string GetDataType(ProcedureParameter procedureParameter, IDictionary<int, CharacterSet> characterSets, int defaultCharacterSetId)
    {
        var builder = new StringBuilder();
        if (procedureParameter.RelationName != null && procedureParameter.FieldName != null)
        {
            builder.Append($"TYPE OF COLUMN {procedureParameter.RelationName.AsSqlIndentifier()}.{procedureParameter.FieldName.AsSqlIndentifier()}");
        }
        else
        {
            switch (procedureParameter.Field.MetadataFieldType)
            {
                case MetadataFieldType.SystemGenerated:
                    builder.Append(GetDataType(procedureParameter.Field, characterSets, defaultCharacterSetId));
                    break;
                case MetadataFieldType.Domain:
                    if (procedureParameter.ParameterMechanism == ProcedureParameterMechanism.TypeOf)
                    {
                        builder.Append("TYPE OF ");
                    }
                    builder.Append(procedureParameter.FieldSource.AsSqlIndentifier());
                    break;
                default:
                    throw new NotSupportedException($"Unknown field type: {procedureParameter.Field.MetadataFieldType}.");
            }
        }
        return builder.ToString();
    }

    public virtual string GetDataType(FunctionArgument functionArgument, IDictionary<int, CharacterSet> characterSets, int defaultCharacterSetId)
    {
        return GetDataType((IDataType)functionArgument, characterSets, defaultCharacterSetId);
    }

    public virtual string GetDataType(RelationField relationField, IDictionary<int, CharacterSet> characterSets, int defaultCharacterSetId)
    {
        switch (relationField.Field.MetadataFieldType)
        {
            case MetadataFieldType.SystemGenerated:
                return GetDataType(relationField.Field, characterSets, defaultCharacterSetId);
            case MetadataFieldType.Domain:
                return relationField.FieldSource.AsSqlIndentifier();
            default:
                throw new NotSupportedException($"Unknown field type: {relationField.Field.MetadataFieldType}.");
        }
    }

    public virtual IEnumerable<Command> HandleAlterCollation(Identifier fieldName, Identifier relationName, IHasCollation collation, IHasCollation otherCollation)
    {
        if (collation.CollationId != null && otherCollation.CollationId != collation.CollationId)
        {
            var systemTableName = relationName != null ? "RDB$RELATION_FIELDS" : "RDB$FIELDS";
            var builder = new StringBuilder();
            builder
                .Append($"UPDATE {systemTableName}")
                .AppendLine()
                .Append($"  SET RDB$COLLATION_ID = {collation.CollationId}")
                .AppendLine()
                .Append($"WHERE RDB$FIELD_NAME = '{fieldName}'");
            if (relationName != null)
            {
                builder.Append($" AND RDB$RELATION_NAME = '{relationName}'");
            }

            yield return new Command().Append(builder.ToString());
        }
    }

    public virtual IEnumerable<Command> HandleAlterDataType<TField>(Func<string, string> alterActionFactory, TField field, TField otherField, IMetadata sourceMetadata, IMetadata targetMetadata)
    {
        var characterSets = sourceMetadata.MetadataCharacterSets.CharacterSetsById;
        var defaultCharacterSet = sourceMetadata.MetadataDatabase.CharacterSet;

        var otherCharacterSets = sourceMetadata.MetadataCharacterSets.CharacterSetsById;
        var otherDefaultCharacterSet = sourceMetadata.MetadataDatabase.CharacterSet;

        var dataType = GetDataType((dynamic)field, characterSets, defaultCharacterSet.CharacterSetId);
        var otherDataType = GetDataType((dynamic)otherField, otherCharacterSets, otherDefaultCharacterSet.CharacterSetId);
        if ((DatabaseStringOrdinal)dataType != (DatabaseStringOrdinal)otherDataType)
        {
            // If we alter a domain, try to keep to original collate
            if ((dynamic)field is Field castedField)
            {
                if (castedField.CharacterSet != null
                    && castedField.CharacterSet.DefaultCollation.CollationId != castedField.CollationId)
                {
                    yield return new Command().Append($"ALTER CHARACTER SET {castedField.CharacterSet.CharacterSetName} SET DEFAULT COLLATION {castedField.Collation.CollationName}");
                    yield return new Command().Append(alterActionFactory($"TYPE {dataType}"));
                    yield return new Command().Append($"ALTER CHARACTER SET {castedField.CharacterSet.CharacterSetName} SET DEFAULT COLLATION {castedField.CharacterSet.DefaultCollateName}");
                    yield break;
                }
            }

            yield return new Command().Append(alterActionFactory($"TYPE {dataType}"));
        }
    }

    public virtual IEnumerable<Command> HandleAlterDefault(Func<string, string> alterActionFactory, IHasDefaultSource @default, IHasDefaultSource otherDefault)
    {
        if (otherDefault.DefaultSource != @default.DefaultSource)
        {
            if (otherDefault.DefaultSource != null)
            {
                yield return new Command().Append(alterActionFactory("DROP DEFAULT"));
            }

            if (@default.DefaultSource != null)
            {
                yield return new Command().Append(alterActionFactory($"SET {@default.DefaultSource}"));
            }
        }
    }

    public virtual IEnumerable<Command> HandleAlterNullable(Identifier fieldName, Identifier relationName, IHasNullable nullable, IHasNullable otherNullable)
    {
        if (otherNullable.Nullable != nullable.Nullable)
        {
            var systemTableName = relationName != null ? "RDB$RELATION_FIELDS" : "RDB$FIELDS";
            var builder = new StringBuilder();
            builder
                .Append($"UPDATE {systemTableName}")
                .AppendLine()
                .Append($"  SET RDB$NULL_FLAG = {(nullable.Nullable ? "NULL" : "1")}")
                .AppendLine()
                .Append($"WHERE RDB$FIELD_NAME = '{fieldName}'");
            if (relationName != null)
            {
                builder.Append($" AND RDB$RELATION_NAME = '{relationName}'");
            }

            yield return new Command().Append(builder.ToString());
        }
    }

    public virtual IEnumerable<Command> HandleAlterValidation(Func<string, string> alterActionFactory, IHasValidationSource validation, IHasValidationSource otherValidation)
    {
        if (otherValidation.ValidationSource != validation.ValidationSource)
        {
            if (otherValidation.ValidationSource != null)
            {
                yield return new Command().Append(alterActionFactory("DROP CONSTRAINT"));
            }

            if (validation.ValidationSource != null)
            {
                yield return new Command().Append(alterActionFactory($"ADD CONSTRAINT {validation.ValidationSource}"));
            }
        }
    }

    public virtual string HandleCollate(Field field, IDictionary<CollationKey, Collation> collations)
    {
        return field.CollationId != null
                   ? Collate(collations, (int)field.CharacterSetId, (int)field.CollationId)
                   : null;
    }

    public virtual string HandleCollate<T>(T item, IDictionary<CollationKey, Collation> collations) where T : IHasCollation, IUsesField
    {
        switch (item.Field.MetadataFieldType)
        {
            case MetadataFieldType.SystemGenerated:
                return item.CollationId != null
                           ? Collate(collations, (int)item.Field.CharacterSetId, (int)item.CollationId)
                           : HandleCollate(item.Field, collations);
            case MetadataFieldType.Domain:
                return item.CollationId != null
                           ? Collate(collations, (int)item.Field.CharacterSetId, (int)item.CollationId)
                           : null;
            default:
                throw new NotSupportedException($"Unknown field type: {item.Field.MetadataFieldType}.");
        }
    }

    public virtual string HandleDefault(IHasDefaultSource item)
    {
        return item.DefaultSource?.ToString();
    }

    public virtual string HandleNullable(IHasNullable item)
    {
        return !item.Nullable ? "NOT NULL" : null;
    }

    public virtual string HandleValidation(IHasValidationSource item)
    {
        return item.ValidationSource?.ToString();
    }

    public virtual bool HasSystemPrefix(Identifier identifier)
    {
        // a way to do an Ordinal match?
        return Regex.IsMatch(identifier.ToString(), @"^RDB\$.+$", RegexOptions.CultureInvariant);
    }

    public virtual bool IsImplicitIntegrityConstraintName(Identifier identifier)
    {
        // a way to do an Ordinal match?
        return Regex.IsMatch(identifier.ToString(), @"^INTEG_\d+$", RegexOptions.CultureInvariant);
    }

    public virtual bool IsValidExternalEngine(IHasExternalEngine item)
    {
        return false;
    }

    /// <summary>
    /// for more information see \src\jrd\obj.h
    /// </summary>
    public virtual string ObjectTypeString(int objectType)
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
            20 => "DATABASE",
            21 => "TABLE",
            22 => "VIEW",
            23 => "PROCEDURE",
            24 => "FUNCTION",
            25 => "PACKAGE",
            26 => "SEQUENCE",
            27 => "DOMAIN",
            28 => "EXCEPTION",
            29 => "ROLE",
            30 => "CHARACTER SET",
            31 => "COLLATION",
            32 => "FILTER",
            _ => throw new ArgumentOutOfRangeException($"Wrong object type: {objectType}."),
        };
    }

    public virtual bool ObjectTypeIsRelation(int objectType) => objectType == 0;
    public virtual bool ObjectTypeIsView(int objectType) => objectType == 1;
    public virtual bool ObjectTypeIsTrigger(int objectType) => objectType == 2;
    public virtual bool ObjectTypeIsField(int objectType) => objectType == 9;
    public virtual bool ObjectTypeIsComputedField(int objectType) => objectType == 3;
    public virtual bool ObjectTypeIsProcedure(int objectType) => objectType == 5;
    public virtual bool ObjectTypeIsException(int objectType) => objectType == 7;
    public virtual bool ObjectTypeIsRole(int objectType) => objectType == 13;
    public virtual bool ObjectTypeIsUser(int objectType) => objectType == 8;
    public virtual bool ObjectTypeIsUDF(int objectType) => objectType == 15;
    public virtual bool ObjectTypeIsExpressionIndex(int objectType) => objectType == 6;
    public virtual bool ObjectTypeIsPackageBody(int objectType) => objectType == 19;
    public virtual bool ObjectTypeIsPackage(int objectType) => objectType == 18;
    public virtual bool ObjectTypeIsGenerator(int objectType) => objectType == 14;
    public virtual bool ObjectTypeIsCharacterSet(int objectType) => objectType == 11;
    public virtual bool ObjectTypeIsCollation(int objectType) => objectType == 17;
    public virtual bool ObjectTypeIsSystemPrivilege(int objectType) => false;

    public virtual string SystemPrivilegeString(int systemPrivilege)
    {
        throw new InvalidOperationException();
    }

    public virtual IEnumerable<Command> HandleAlterIdentity(RelationField field, RelationField otherField)
    {
        if (field.IdentityType != otherField.IdentityType)
        {
            throw new NotSupportedOnFirebirdException($"Altering identity definition on a field is not supported ({field.RelationName}.{field.FieldName}).");
        }
        yield break;
    }

    public virtual string SqlSecurityString(bool? sqlSecurity)
    {
        throw new InvalidOperationException();
    }

    protected virtual string ScalableNumber(string regularType, IDataType dataType)
    {
        if (dataType.FieldSubType == 2 || dataType.FieldSubType == 0 && dataType.FieldScale < 0)
            return $"DECIMAL({Precision(dataType)},{-dataType.FieldScale})";
        if (dataType.FieldSubType == 1)
            return $"NUMERIC({Precision(dataType)},{-dataType.FieldScale})";
        return regularType;
    }

    protected virtual string Collate(IDictionary<CollationKey, Collation> collations, int characterSetId, int collationId)
    {
        var collate = collations[new CollationKey(characterSetId, collationId)];
        return collate.CharacterSet.DefaultCollation.CollationId != collationId
                   ? $"COLLATE {collate.CollationName}"
                   : null;
    }

    protected virtual string VarcharChar(IDataType dataType, IDictionary<int, CharacterSet> characterSets, int defaultCharacterSetId)
    {
        var builder = new StringBuilder();
        switch (dataType.FieldType)
        {
            case FieldType.Varying:
                builder.Append("VARCHAR");
                break;
            case FieldType.Text:
                builder.Append("CHAR");
                break;
            default:
                throw new ArgumentOutOfRangeException($"Wrong type: {dataType.FieldType}.");
        }
        var characterSet = characterSets[dataType.CharacterSetId ?? defaultCharacterSetId];
        var length = dataType.FieldLength;
        if (dataType.CharacterLength != null)
        {
            if (dataType.CharacterLength > 0)
            {
                length = dataType.CharacterLength;
            }
            else
            {
                length = dataType.FieldLength / characterSet.BytesPerCharacter;
            }
        }
        builder.Append($"({length})");
        if (dataType.CharacterSetId != defaultCharacterSetId)
        {
            builder.Append($" CHARACTER SET {characterSet.CharacterSetName}");
        }
        return builder.ToString();
    }

    protected virtual int? Precision(IDataType dataType)
    {
        return dataType.FieldPrecision != 0
            ? dataType.FieldPrecision
            : 18;
    }
}
