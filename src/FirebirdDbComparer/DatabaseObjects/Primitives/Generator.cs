using System.Collections.Generic;
using System.Diagnostics;
using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Compare;
using FirebirdDbComparer.Exceptions;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Primitives;

public sealed class Generator : Primitive<Generator>, IHasSystemFlag, IHasDescription
{
    private static readonly EquatableProperty<Generator>[] s_EquatableProperties =
    {
            new EquatableProperty<Generator>(x => x.GeneratorName, nameof(GeneratorName)),
            new EquatableProperty<Generator>(x => x.SystemFlag, nameof(SystemFlag)),
            new EquatableProperty<Generator>(x => x.OwnerName, nameof(OwnerName)),
            //new EquatableProperty<Generator>(x => x.InitialValue, nameof(InitialValue)),
            new EquatableProperty<Generator>(x => x.GeneratorIncrement, nameof(GeneratorIncrement))
        };

    public Generator(ISqlHelper sqlHelper)
        : base(sqlHelper)
    { }

    public Identifier GeneratorName { get; private set; }
    public int GeneratorId { get; private set; }
    public SystemFlagType SystemFlag { get; private set; }
    public DatabaseStringOrdinal Description { get; private set; }
    public Identifier OwnerName { get; private set; }
    public long? InitialValue { get; private set; }
    public int? GeneratorIncrement { get; private set; }

    protected override Generator Self => this;

    protected override EquatableProperty<Generator>[] EquatableProperties => s_EquatableProperties;

    protected override IEnumerable<Command> OnCreate(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
    {
        var command = new Command();
        command.Append($"CREATE SEQUENCE {GeneratorName.AsSqlIndentifier()}");
        if (context.Settings.TargetVersion.AtLeast(TargetVersion.Version30))
        {
            if (InitialValue != 0)
            {
                command.Append($" START WITH {InitialValue}");
            }
            if (GeneratorIncrement != 1)
            {
                command.Append($" INCREMENT BY {GeneratorIncrement}");
            }
        }
        yield return command;
    }

    protected override IEnumerable<Command> OnDrop(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
    {
        yield return new Command()
            .Append($"DROP SEQUENCE {GeneratorName.AsSqlIndentifier()}");
    }

    protected override IEnumerable<Command> OnAlter(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
    {
        var otherGenerator = FindOtherChecked(targetMetadata.MetadataGenerators.GeneratorsByName, GeneratorName, "role");

        if (EquatableHelper.PropertiesEqual(this, otherGenerator, EquatableProperties, nameof(GeneratorIncrement)))
        {
            yield return new Command()
                .Append($"ALTER SEQUENCE {GeneratorName.AsSqlIndentifier()} INCREMENT BY {GeneratorIncrement}");
        }
        else
        {
            throw new NotSupportedOnFirebirdException($"Altering sequence is not supported ({GeneratorName}).");
        }
    }

    protected override Identifier OnPrimitiveTypeKeyObjectName() => GeneratorName;

    internal static Generator CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values)
    {
        var result =
            new Generator(sqlHelper)
            {
                GeneratorName = new Identifier(sqlHelper, values["RDB$GENERATOR_NAME"].DbValueToString()),
                GeneratorId = values["RDB$GENERATOR_ID"].DbValueToInt32().GetValueOrDefault(),
                SystemFlag = (SystemFlagType)values["RDB$SYSTEM_FLAG"].DbValueToInt32().GetValueOrDefault(),
                Description = values["RDB$DESCRIPTION"].DbValueToString(),
            };

        if (sqlHelper.TargetVersion.AtLeast(TargetVersion.Version30))
        {
            result.OwnerName = new Identifier(sqlHelper, values["RDB$OWNER_NAME"].DbValueToString());
            result.InitialValue = values["RDB$INITIAL_VALUE"].DbValueToInt64();
            result.GeneratorIncrement = values["RDB$GENERATOR_INCREMENT"].DbValueToInt32();
        }
        return result;
    }
}
