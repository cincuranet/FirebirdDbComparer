using System.Collections.Generic;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;
using FirebirdDbComparer.Compare;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Primitives
{
    public sealed class DbException : Primitive<DbException>, IHasSystemFlag, IHasDescription
    {
        private static readonly EquatableProperty<DbException>[] s_EquatableProperties =
        {
            new EquatableProperty<DbException>(x => x.ExceptionName, nameof(ExceptionName)),
            new EquatableProperty<DbException>(x => x.Message, nameof(Message)),
            new EquatableProperty<DbException>(x => x.SystemFlag, nameof(SystemFlag)),
            new EquatableProperty<DbException>(x => x.OwnerName, nameof(OwnerName))
        };

        public DbException(ISqlHelper sqlHelper)
            : base(sqlHelper)
        { }

        public Identifier ExceptionName { get; private set; }
        public int ExceptionNumber { get; private set; }
        public DatabaseStringOrdinal Message { get; private set; }
        public DatabaseStringOrdinal Description { get; private set; }
        public SystemFlagType SystemFlag { get; private set; }
        public Identifier OwnerName { get; private set; }

        protected override DbException Self => this;

        protected override EquatableProperty<DbException>[] EquatableProperties => s_EquatableProperties;

        protected override IEnumerable<Command> OnCreate(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            yield return new Command()
                .Append($"CREATE OR ALTER EXCEPTION {ExceptionName.AsSqlIndentifier()} '{SqlHelper.DoubleSingleQuotes(Message)}'");
        }

        protected override IEnumerable<Command> OnDrop(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            yield return new Command()
                .Append($"DROP EXCEPTION {ExceptionName.AsSqlIndentifier()}");
        }

        protected override IEnumerable<Command> OnAlter(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            return OnCreate(sourceMetadata, targetMetadata, context);
        }

        protected override Identifier OnPrimitiveTypeKeyObjectName() => ExceptionName;

        internal static DbException CreateFrom(ISqlHelper sqlHelper, IDictionary<string, object> values)
        {
            var result =
                new DbException(sqlHelper)
                {
                    ExceptionName = new Identifier(sqlHelper, values["RDB$EXCEPTION_NAME"].DbValueToString()),
                    ExceptionNumber = values["RDB$EXCEPTION_NUMBER"].DbValueToInt32().GetValueOrDefault(),
                    Message = values["RDB$MESSAGE"].DbValueToString(),
                    Description = values["RDB$DESCRIPTION"].DbValueToString(),
                    SystemFlag = (SystemFlagType)values["RDB$SYSTEM_FLAG"].DbValueToInt32().GetValueOrDefault()
                };

            if (sqlHelper.TargetVersion.AtLeast(TargetVersion.Version30))
            {
                result.OwnerName = new Identifier(sqlHelper, values["RDB$OWNER_NAME"].DbValueToString());
            }
            return result;
        }
    }
}
