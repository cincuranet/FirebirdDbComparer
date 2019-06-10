using System;
using System.Collections.Generic;

using FirebirdDbComparer.Compare;
using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.Elements;
using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Interfaces
{
    public interface ISqlHelper
    {
        TargetVersion TargetVersion { get; }
        string Terminator { get; }
        string AlternativeTerminator { get; }
        DatabaseStringOrdinal[] Keywords { get; }
        string DoubleSingleQuotes(string value);
        string DoubleSingleQuotes(DatabaseStringOrdinal value);
        string QuoteIdentifierIfNeeded(string value);
        string QuoteIdentifierIfNeeded(DatabaseStringOrdinal value);
        string CreateComment(string objectTypeName, IEnumerable<Identifier> objectNames, DatabaseStringOrdinal description, DatabaseStringOrdinal otherDescription);
        string GetDataType(IDataType dataType, IDictionary<int, CharacterSet> characterSets, int defaultCharacterSetId);
        string GetDataType(ProcedureParameter procedureParameter, IDictionary<int, CharacterSet> characterSets, int defaultCharacterSetId);
        string GetDataType(FunctionArgument functionArgument, IDictionary<int, CharacterSet> characterSets, int defaultCharacterSetId);
        string GetDataType(RelationField relationField, IDictionary<int, CharacterSet> characterSets, int defaultCharacterSetId);
        IEnumerable<Command> HandleAlterCollation(Identifier fieldName, Identifier relationName, IHasCollation collation, IHasCollation otherCollation);
        IEnumerable<Command> HandleAlterDataType<TField>(Func<string, string> alterActionFactory, TField field, TField otherField, IMetadata sourceMetadata, IMetadata targetMetadata);
        IEnumerable<Command> HandleAlterDefault(Func<string, string> alterActionFactory, IHasDefaultSource @default, IHasDefaultSource otherDefault);
        IEnumerable<Command> HandleAlterNullable(Identifier fieldName, Identifier relationName, IHasNullable nullable, IHasNullable otherNullable);
        IEnumerable<Command> HandleAlterValidation(Func<string, string> alterActionFactory, IHasValidationSource validation, IHasValidationSource otherValidation);
        string HandleCollate(Field field, IDictionary<CollationKey, Collation> collations);
        string HandleCollate<T>(T item, IDictionary<CollationKey, Collation> collations) where T : IHasCollation, IUsesField;
        string HandleDefault(IHasDefaultSource item);
        string HandleNullable(IHasNullable item);
        string HandleValidation(IHasValidationSource item);
        bool HasSystemPrefix(Identifier identifier);
        bool IsImplicitIntegrityConstraintName(Identifier identifier);
        bool IsValidExternalEngine(IHasExternalEngine item);
    }
}
