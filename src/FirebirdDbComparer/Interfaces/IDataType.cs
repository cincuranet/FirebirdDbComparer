using FirebirdDbComparer.DatabaseObjects;

namespace FirebirdDbComparer.Interfaces
{
    public interface IDataType
    {
        FieldType FieldType { get; }
        int? FieldSubType { get; }
        int? FieldScale { get; }
        int? FieldPrecision { get; }
        int? FieldLength { get; }
        int? SegmentSize { get; }
        int? CharacterLength { get; }
        int? CharacterSetId { get; }
        int? CollationId { get; }
    }
}
