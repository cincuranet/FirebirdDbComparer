using System;
using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

using FirebirdSql.Data.FirebirdClient;

namespace FirebirdDbComparer.DatabaseObjects
{
    public abstract class DatabaseObject : IDatabaseObject
    {
        public ISqlHelper SqlHelper { get; }

        protected DatabaseObject(IMetadata metadata, ISqlHelper sqlHelper)
        {
            SqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof(sqlHelper));
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        }

        protected IMetadata Metadata { get; }
        protected string ConnectionString => Metadata?.ConnectionString;
        public abstract void Initialize();
        public abstract void FinishInitialization();

        protected IEnumerable<IDictionary<string, object>> Execute(string commandText)
        {
            using (var connection = new FbConnection(ConnectionString))
            {
                connection.Open();
                var fbTransactionOptions =
                    new FbTransactionOptions
                    {
                        TransactionBehavior = FbTransactionBehavior.Read | FbTransactionBehavior.ReadCommitted | FbTransactionBehavior.RecVersion | FbTransactionBehavior.NoWait
                    };
                using (var transaction = connection.BeginTransaction(fbTransactionOptions))
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = commandText;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var values = new object[reader.FieldCount];
                                reader.GetValues(values);
                                var result = new Dictionary<string, object>();
                                for (var i = 0; i < reader.FieldCount; i++)
                                {
                                    result.Add(reader.GetName(i), values[i]);
                                }
                                yield return result;
                            }
                        }
                    }
                }
            }
        }

        protected IEnumerable<Command> HandleComment<TPrimitive>(
            IDictionary<Identifier, TPrimitive> primitives,
            IDictionary<Identifier, TPrimitive> otherPrimitives,
            Func<TPrimitive, Identifier> keySelector,
            string objectTypeName,
            Func<TPrimitive, IEnumerable<Identifier>> nameFactory,
            IComparerContext context,
            Func<TPrimitive, IEnumerable<Command>> nestedFactory = null,
            Func<TPrimitive, bool> primitivesFilter = null)
            where TPrimitive : IHasSystemFlag, IHasDescription
        {
            var data = FilterSystemFlagUser(primitives.Values);
            if (primitivesFilter != null)
            {
                data = data.Where(primitivesFilter);
            }

            foreach (var primitive in data)
            {
                var key = keySelector(primitive);
                otherPrimitives.TryGetValue(key, out var other);

                var comment = SqlHelper.CreateComment(
                    objectTypeName,
                    nameFactory(primitive),
                    primitive.Description,
                    other?.Description);
                if (comment != null)
                {
                    yield return new Command().Append(comment);
                }

                if (nestedFactory != null)
                {
                    foreach (var item in nestedFactory(primitive))
                    {
                        yield return item;
                    }
                }
            }
        }

        protected IEnumerable<Command> HandleCommentNested<TNested, TEquatable>(
            IEnumerable<TNested> nesteds,
            IDictionary<TEquatable, TNested> otherNesteds,
            Func<Identifier, Identifier, TEquatable> equatableFactory,
            Identifier parentName,
            Func<TNested, Identifier> nestedKeySelector,
            string objectTypeName,
            Func<TNested, IEnumerable<Identifier>> nestedNameFactory,
            IComparerContext context)
            where TNested : IHasDescription
        {
            foreach (var nested in nesteds)
            {
                var key = equatableFactory(parentName, nestedKeySelector(nested));
                otherNesteds.TryGetValue(key, out var other);

                var comment = SqlHelper.CreateComment(
                    objectTypeName,
                    new[] { parentName }.Concat(nestedNameFactory(nested)),
                    nested.Description,
                    other?.Description);
                if (comment != null)
                {
                    yield return new Command().Append(comment);
                }
            }
        }

        protected static Func<IMetadata, IMetadata, IComparerContext, IEnumerable<Command>> WrapActionWithEmptyBody(Func<IMetadata, IMetadata, IComparerContext, IEnumerable<Command>> action)
        {
            return (sourceMetadata, targetMetadata, context) => WrapActionWithEmptyBodyWrapper(action, sourceMetadata, targetMetadata, context);
        }

        protected static bool FilterSystemFlagUserPredicate<T>(T item) where T : IHasSystemFlag
        {
            return item.SystemFlag == SystemFlagType.USER;
        }

        protected static IEnumerable<T> FilterSystemFlagUser<T>(IEnumerable<T> source) where T : IHasSystemFlag
        {
            return source.Where(FilterSystemFlagUserPredicate);
        }

        private static IEnumerable<Command> WrapActionWithEmptyBodyWrapper(Func<IMetadata, IMetadata, IComparerContext, IEnumerable<Command>> action, IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            context.EnableEmptyBodies();
            try
            {
                foreach (var item in action(sourceMetadata, targetMetadata, context))
                {
                    yield return item;
                }
            }
            finally
            {
                context.DisableEmptyBodies();
            }
        }
    }
}
